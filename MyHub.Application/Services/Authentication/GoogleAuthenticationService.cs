using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using MyHub.Domain.Authentication;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Authentication.Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users;
using MyHub.Domain.Enums.Enumerations;
using Google.Apis.Util;
using MyHub.Domain.Authentication.Claims;
using MyHub.Domain.Authentication.Interfaces;

namespace MyHub.Application.Services.Authentication
{
	public class GoogleAuthenticationService : IGoogleAuthenticationService
	{
		private readonly IUsersService _usersService;
		private readonly IGoogleUsersService _googleUsersService;
		private readonly IAuthenticationService _authenticationService;
		private readonly AuthenticationOptions _authOptions;
		private readonly IClock _clock = SystemClock.Default;

		public GoogleAuthenticationService(IUsersService hubUsersService, IGoogleUsersService googleUsersService, IAuthenticationService authenticationService, IOptions<AuthenticationOptions> authOptions)
		{
			_usersService = hubUsersService;
			_googleUsersService = googleUsersService;
			_authenticationService = authenticationService;
			_authOptions = authOptions.Value;
		}

		public Validator<TokenResponse> RefreshUserAuthentication(string userId, string refreshToken)
		{
			try
			{
				//If google call fails, can use this to refresh before making call again.

				var user = _usersService.GetFullAccessingUserById(userId);

				if (user is null)
					return new Validator<TokenResponse>().AddError("User does not exist.");

				var tokenResponse = RefreshToken(refreshToken, user).Result;

				if (tokenResponse is null || tokenResponse.IsExpired(_clock) || string.IsNullOrWhiteSpace(tokenResponse.IdToken)
					|| string.IsNullOrWhiteSpace(tokenResponse.AccessToken) || string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
				{
					_usersService.RevokeUser(user, refreshToken);

					return new Validator<TokenResponse>().AddError("Your login session has expired.");
				}

				return new Validator<TokenResponse>(tokenResponse);
			}
			catch (Exception)
			{
				return new Validator<TokenResponse>().AddError("Your login session has expired.");
			}
		}

		private async Task<TokenResponse?> RefreshToken(string refreshToken, AccessingUser user)
		{
			try
			{
				return await GetCodeFlow().RefreshTokenAsync(user.Id, refreshToken, CancellationToken.None);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private GoogleAuthorizationCodeFlow GetCodeFlow()
		{
			return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = _authOptions.ThirdPartyLogin.Google.ClientId,
					ClientSecret = _authOptions.ThirdPartyLogin.Google.ClientSecret
				},
				Scopes = new[] { "profile", "email", "openid" }
			});
		}

		private Tokens GenerateTokens(GoogleJsonWebSignature.Payload payload)
		{
			return _authenticationService.GenerateTokens(new HubClaims
			{
				Sub = payload.Subject,
				Email = payload.Email,
				Name = payload.Name,
				Iss = _authOptions.JWT.Issuer,
				IssManaging = LoginIssuers.Google.Id,
				Aud = _authOptions.JWT.Audience,
				FamilyName = payload.FamilyName,
				GivenName = payload.GivenName
			});
		}

		public async Task<Validator<GoogleUser>> ExchangeAuthCode(string authUser, string authCode, string nonce)
		{
			var tokenResponse = await GetCodeFlow().ExchangeCodeForTokenAsync(authUser, authCode, _authOptions.ThirdPartyLogin.Google.RedirectUri, CancellationToken.None);

			var payload = GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new List<string> { _authOptions.ThirdPartyLogin.Google.Audience } }).Result;

			if (nonce != payload.Nonce)
				return new Validator<GoogleUser>().AddError("Nonce is invalid. Cannot login.");

			var hubTokens = GenerateTokens(payload);

			var createOrUpdate = await CreateOrUpdateUser(payload, hubTokens);

			if (createOrUpdate.IsInvalid)
				return createOrUpdate;

			var googleUser = new GoogleUser
			{
				Email = payload.Email,
				Username = payload.Name,
				IdToken = hubTokens.IdToken,
				AccessToken = tokenResponse.AccessToken,
				RefreshToken = hubTokens.RefreshToken
			};

			return new Validator<GoogleUser>(googleUser);
		}

		private async Task<Validator<GoogleUser>> CreateOrUpdateUser(GoogleJsonWebSignature.Payload payload, Tokens tokens)
		{
			var hubUser = _usersService.GetFullAccessingUserByEmail(payload.Email);

			if (hubUser is not null)
			{
				if (hubUser.ThirdPartyDetails.ThirdPartyIssuerId != LoginIssuers.Google.Id)
					return new Validator<GoogleUser>().AddError("Your email address is already associated with Marco's Hub.");

				hubUser.Email = payload.Email;
				_usersService.AddRefreshToken(hubUser, tokens.RefreshToken);
			}
			else
			{
				var user = new AccessingUser
				{
					User = new User { Id = payload.Subject, Name = payload.GivenName ?? string.Empty, Surname = payload.FamilyName ?? string.Empty, Username = payload.Name },
					Email = payload.Email,
					ThirdPartyDetails = new ThirdPartyDetails
					{
						ThirdPartyIssuerId = LoginIssuers.Google.Id
					},
					RefreshTokens = new List<RefreshToken> { new RefreshToken { Id = Guid.NewGuid().ToString(), Token = tokens.RefreshToken, CreatedDate = DateTime.Now } }
				};

				var uploaded = await _usersService.UpdateUserProfileImage(user.User.Id, await _googleUsersService.GetUserProfileImage(payload.Picture));

				if (uploaded)
					_usersService.RegisterThirdParty(user);
				else
					return new Validator<GoogleUser>().AddError("Unable to create user.");
			}

			return new Validator<GoogleUser>();
		}
	}
}
