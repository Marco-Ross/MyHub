using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Claims;
using MyHub.Domain.Authentication.Github;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Github;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using Octokit;

namespace MyHub.Application.Services.Authentication
{
	public partial class GithubAuthenticationService : IGithubAuthenticationService
	{
		private readonly IUsersService _usersService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IGithubUsersService _githubUsersService;
		private readonly AuthenticationOptions _authOptions;
		private readonly GitHubClient _githubClient;

		public GithubAuthenticationService(IUsersService hubUsersService, IAuthenticationService authenticationService, IOptions<AuthenticationOptions> authOptions, IGithubUsersService githubUsersService)
		{
			_usersService = hubUsersService;
			_authenticationService = authenticationService;
			_authOptions = authOptions.Value;
			_githubClient = new GitHubClient(new ProductHeaderValue("MarcosHub"));
			_githubUsersService = githubUsersService;
		}

		private Tokens GenerateTokens(Octokit.User githubUser, EmailAddress githubEmail)
		{
			return _authenticationService.GenerateTokens(new HubClaims
			{
				Sub = githubUser.Id.ToString(),
				Email = githubEmail.Email,
				Name = githubUser.Name,
				Iss = _authOptions.JWT.Issuer,
				IssManaging = LoginIssuers.Github.Id,
				Aud = _authOptions.JWT.Audience
			});
		}

		public async Task<Validator<GithubUser>> ExchangeAuthCode(string authCode)
		{
			var githubAccessResponse = await GetAccessToken(authCode);

			if (githubAccessResponse is null)
				return new Validator<GithubUser>().AddError("Exchanging access token failed.");

			if (!string.IsNullOrWhiteSpace(githubAccessResponse?.Error))
				return new Validator<GithubUser>().AddError($"Failed to exchange code for tokens: {githubAccessResponse.Error}");

			return await SetUserDetails(githubAccessResponse);
		}

		private async Task<Validator<GithubUser>> SetUserDetails(OauthToken? githubAccessResponse)
		{
			if (githubAccessResponse is null)
				return new Validator<GithubUser>().AddError("Exchanging access token failed.");

			SetUserCredentials(githubAccessResponse);

			var githubUserResponse = await _githubClient.User.Current();
			var githubUserEmailResponse = await _githubClient.User.Email.GetAll();

			if (githubUserResponse is null || githubUserEmailResponse is null)
				return new Validator<GithubUser>().AddError("Failed to get user details.");

			var userEmail = githubUserEmailResponse.Where(x => x.Primary)?.FirstOrDefault();

			if (userEmail is null)
				return new Validator<GithubUser>().AddError("Your email address is not accessible from github.");

			var hubTokens = GenerateTokens(githubUserResponse, userEmail);

			var createOrUpdate = await CreateOrUpdateUser(githubUserResponse, userEmail, hubTokens);

			if (createOrUpdate.IsInvalid)
				return createOrUpdate;

			var githubUser = new GithubUser
			{
				Email = userEmail.Email,
				Username = githubUserResponse.Name,
				IdToken = hubTokens.IdToken,
				AccessToken = githubAccessResponse.AccessToken,
				RefreshToken = hubTokens.RefreshToken,
			};

			return new Validator<GithubUser>(githubUser);
		}

		private void SetUserCredentials(OauthToken githubAccessResponse)
		{
			_githubClient.Credentials = new Credentials(githubAccessResponse.AccessToken); //Only for auth service. Will be injected elsewhere
		}

		private async Task<Validator<GithubUser>> CreateOrUpdateUser(Octokit.User githubUser, EmailAddress githubEmail, Tokens hubTokens)
		{
			var hubUser = _usersService.GetFullAccessingUserByEmail(githubEmail.Email);

			if (hubUser is not null)
			{
				if (hubUser.ThirdPartyDetails.ThirdPartyIssuerId != LoginIssuers.Github.Id)
					return new Validator<GithubUser>().AddError("Your email address is already associated with Marco's Hub.");

				hubUser.User.Email = githubEmail.Email;
				_usersService.AddRefreshToken(hubUser, hubTokens.RefreshToken);
			}
			else
			{
				var user = new AccessingUser
				{
					User = new Domain.Users.User { Id = githubUser.Id.ToString(), Email = githubEmail.Email, Username = githubUser.Name },
					ThirdPartyDetails = new ThirdPartyDetails
					{
						ThirdPartyIssuerId = LoginIssuers.Github.Id
					},
					RefreshTokens = new List<RefreshToken> { new RefreshToken { Id = Guid.NewGuid().ToString(), Token = hubTokens.RefreshToken, CreatedDate = DateTime.Now } }
				};

				var uploaded = await _usersService.UpdateUserProfileImage(user.User.Id, await _githubUsersService.GetUserProfileImage(githubUser.AvatarUrl));

				if (uploaded)
					_usersService.RegisterThirdParty(user);
				else
					return new Validator<GithubUser>().AddError("Unable to create user.");
			}

			return new Validator<GithubUser>();
		}

		private async Task<OauthToken?> GetAccessToken(string authCode)
		{
			try
			{
				return await _githubClient.Oauth.CreateAccessToken(new OauthTokenRequest(_authOptions.ThirdPartyLogin.Github.ClientId, _authOptions.ThirdPartyLogin.Github.ClientSecret, authCode));
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
