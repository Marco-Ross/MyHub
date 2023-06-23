using Facebook;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Claims;
using MyHub.Domain.Authentication.Facebook;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Facebook;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using System.Text.Json;

namespace MyHub.Application.Services.Authentication
{
	public partial class FacebookAuthenticationService : IFacebookAuthenticationService
	{
		private readonly IUsersService _usersService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IFacebookUsersService _facebookUsersService;
		private readonly AuthenticationOptions _authOptions;
		private readonly FacebookClient _facebookClient;

		public FacebookAuthenticationService(IUsersService hubUsersService, IAuthenticationService authenticationService, IOptions<AuthenticationOptions> authOptions, IFacebookUsersService facebookUsersService)
		{
			_usersService = hubUsersService;
			_authenticationService = authenticationService;
			_authOptions = authOptions.Value;
			_facebookClient = new FacebookClient();
			_facebookUsersService = facebookUsersService;
		}

		private Tokens GenerateTokens(FacebookDetailsResponse facebookResponse)
		{
			return _authenticationService.GenerateTokens(new HubClaims
			{
				Sub = facebookResponse.Id,
				Email = facebookResponse.Email,
				Name = facebookResponse.Name,
				Iss = _authOptions.JWT.Issuer,
				IssManaging = LoginIssuers.Facebook.Id,
				Aud = _authOptions.JWT.Audience,
				FamilyName = facebookResponse.LastName,
				GivenName = facebookResponse.FirstName
			});
		}

		public async Task<Validator<FacebookUser>> ExchangeAuthCode(string authCode)
		{
			var facebookAccessResponse = await GetAccessToken(authCode);

			if (facebookAccessResponse is null)
				return new Validator<FacebookUser>().AddError("Exchanging access token failed.");

			if (!string.IsNullOrWhiteSpace(facebookAccessResponse?.Error))
				return new Validator<FacebookUser>().AddError($"Failed to exchange code for tokens: {facebookAccessResponse.Error}");

			return await ExchangeAccessToken(facebookAccessResponse);
		}

		private async Task<Validator<FacebookUser>> ExchangeAccessToken(FacebookAccessResponse? facebookAccessResponse)
		{
			if (facebookAccessResponse is null)
				return new Validator<FacebookUser>().AddError("Exchanging access token failed.");

			var facebookLongLivedAccessResponse = await GetLongLivedAccessToken(facebookAccessResponse);

			if (facebookLongLivedAccessResponse is null)
				return new Validator<FacebookUser>().AddError("Exchanging for long lived access token failed.");

			var facebookResponse = await _facebookUsersService.GetUserDetails(facebookLongLivedAccessResponse.AccessToken);

			if (facebookResponse is null)
				return new Validator<FacebookUser>().AddError("Failed to get user details.");

			var hubTokens = GenerateTokens(facebookResponse);

			var createOrUpdate = await CreateOrUpdateUser(facebookResponse, hubTokens);

			if (createOrUpdate.IsInvalid)
				return createOrUpdate;

			var facebookUser = new FacebookUser
			{
				Email = facebookResponse.Email,
				Username = facebookResponse.Name,
				IdToken = hubTokens.IdToken,
				AccessToken = facebookLongLivedAccessResponse.AccessToken,
				RefreshToken = hubTokens.RefreshToken,
			};

			return new Validator<FacebookUser>(facebookUser);
		}

		private async Task<Validator<FacebookUser>> CreateOrUpdateUser(FacebookDetailsResponse facebookResponse, Tokens hubTokens)
		{
			var hubUser = _usersService.GetFullAccessingUserByEmail(facebookResponse.Email);

			if (hubUser is not null)
			{
				if (hubUser.ThirdPartyDetails.ThirdPartyIssuerId != LoginIssuers.Facebook.Id)
					return new Validator<FacebookUser>().AddError("Your email address is already associated with Marco's Hub.");

				hubUser.User.Email = facebookResponse.Email;
				_usersService.AddRefreshToken(hubUser, hubTokens.RefreshToken);
			}
			else
			{
				var user = new AccessingUser
				{
					User = new User { Id = facebookResponse.Id, Email = facebookResponse.Email, Name = facebookResponse.FirstName ?? string.Empty, Surname = facebookResponse.LastName ?? string.Empty, Username = facebookResponse.Name },
					ThirdPartyDetails = new ThirdPartyDetails
					{
						ThirdPartyIssuerId = LoginIssuers.Facebook.Id
					},
					RefreshTokens = new List<RefreshToken> { new RefreshToken { Id = Guid.NewGuid().ToString(), Token = hubTokens.RefreshToken, CreatedDate = DateTime.Now } }
				};

				var uploaded = await _usersService.UpdateUserProfileImage(user.User.Id, await _facebookUsersService.GetUserProfileImage(facebookResponse.PictureData.Picture.Url));

				if (uploaded)
					_usersService.RegisterThirdParty(user);
				else
					return new Validator<FacebookUser>().AddError("Unable to create user.");
			}

			return new Validator<FacebookUser>();
		}

		private async Task<FacebookAccessResponse?> GetAccessToken(string authCode)
		{
			try
			{
				var parameters = new Dictionary<string, object>
				{
					["client_id"] = _authOptions.ThirdPartyLogin.Facebook.ClientId,
					["client_secret"] = _authOptions.ThirdPartyLogin.Facebook.ClientSecret,
					["redirect_uri"] = _authOptions.ThirdPartyLogin.Facebook.RedirectUri,
					["code"] = authCode
				};

				var resultTask = await _facebookClient.GetTaskAsync("oauth/access_token", parameters);
				return JsonSerializer.Deserialize<FacebookAccessResponse>(resultTask.ToString() ?? string.Empty);

			}
			catch (Exception)
			{
				return null;
			}
		}

		private async Task<FacebookAccessResponse?> GetLongLivedAccessToken(FacebookAccessResponse? facebookAccessResponse)
		{
			try
			{
				var parameters = new Dictionary<string, object>
				{
					["grant_type"] = "fb_exchange_token",
					["client_id"] = _authOptions.ThirdPartyLogin.Facebook.ClientId,
					["client_secret"] = _authOptions.ThirdPartyLogin.Facebook.ClientSecret,
					["fb_exchange_token"] = facebookAccessResponse?.AccessToken ?? string.Empty
				};

				var longLivedResponse = await _facebookClient.GetTaskAsync("oauth/access_token", parameters);
				return JsonSerializer.Deserialize<FacebookAccessResponse>(longLivedResponse.ToString() ?? string.Empty);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
