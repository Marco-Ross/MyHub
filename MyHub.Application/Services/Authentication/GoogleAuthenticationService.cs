using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using MyHub.Domain.Authentication;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Infrastructure.Repository.EntityFramework;
using MyHub.Domain.Users.UsersDto;
using MyHub.Domain.Authentication.Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users;
using MyHub.Domain.Enums.Enumerations;

namespace MyHub.Application.Services.Authentication
{
	public class GoogleAuthenticationService : IGoogleAuthenticationService
	{
		private readonly IUsersService _hubUsersService;
		private readonly AuthenticationOptions _authOptions;
		private readonly ApplicationDbContext _applicationDbContext;

		public GoogleAuthenticationService(ApplicationDbContext applicationDbContext, IUsersService hubUsersService, IOptions<AuthenticationOptions> authOptions)
		{
			_applicationDbContext = applicationDbContext;
			_hubUsersService = hubUsersService;
			_authOptions = authOptions.Value;
		}

		public Validator<LoginDetails> RefreshUserAuthentication(string idToken, string refreshToken)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(idToken);

			var user = _hubUsersService.GetFullAccessingUserById(jwtToken.Subject);

			if (user is null)
				return new Validator<LoginDetails>().AddError("User does not exist.");

			var tokenResponse = RefreshToken(refreshToken, user).Result;

			user.ThirdPartyIdToken = tokenResponse.IdToken;
			user.ThirdPartyAccessToken = tokenResponse.AccessToken;
			_hubUsersService.UpdateRefreshToken(user, refreshToken, tokenResponse.RefreshToken);


			var newJwtToken = tokenHandler.ReadJwtToken(tokenResponse.IdToken);

			// see if i can make this more sharable with the login. since login issuer is set here and on the login.
			//Cleanup code in here.
			var loginDetails = new LoginDetails
			{
				Tokens = new Tokens { IdToken = tokenResponse.IdToken, RefreshToken = tokenResponse.RefreshToken },
				HubUserDto = new HubUserDto
				{
					Email = newJwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty,
					Username = newJwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value ?? string.Empty,
					LoginIssuer = LoginIssuers.Google.Name
				}
			};

			return new Validator<LoginDetails>().Response(loginDetails);
		}

		private async Task<TokenResponse> RefreshToken(string refreshToken, AccessingUser user)
		{
			var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = _authOptions.ThirdPartyLogin.Google.ClientId,
					ClientSecret = _authOptions.ThirdPartyLogin.Google.ClientSecret
				}
			});

			var tokenResponse = await flow.RefreshTokenAsync(user.Id, refreshToken, CancellationToken.None);
			return tokenResponse;
		}

		public async Task<GoogleUser> ExchangeAuthCode(string authUser, string authCode)
		{
			var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = _authOptions.ThirdPartyLogin.Google.ClientId,
					ClientSecret = _authOptions.ThirdPartyLogin.Google.ClientSecret
				},
				Scopes = new[] { "profile", "email", "openid" },
				Prompt = "consent"
			});

			var tokenResponse = await flow.ExchangeCodeForTokenAsync(authUser, authCode, _authOptions.ThirdPartyLogin.Google.RedirectUri, CancellationToken.None);

			return CreateOrUpdateUser(tokenResponse);
		}

		private GoogleUser CreateOrUpdateUser(TokenResponse tokenResponse)
		{
			var payload = GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new List<string> { _authOptions.ThirdPartyLogin.Google.Audience } }).Result;

			var googleUser = _hubUsersService.GetFullAccessingUserById(payload.Subject);

			if (googleUser is not null)
			{
				googleUser.Email = payload.Email;
				googleUser.ThirdPartyIdToken = tokenResponse.IdToken;
				googleUser.ThirdPartyAccessToken = tokenResponse.AccessToken;
				_hubUsersService.AddRefreshToken(googleUser, tokenResponse.RefreshToken);
			}
			else
			{
				var user = new AccessingUser
				{
					User = new User { Id = payload.Subject },
					Email = payload.Email,
					ThirdPartyIdToken = tokenResponse.IdToken,
					ThirdPartyAccessToken = tokenResponse.AccessToken,
					RefreshTokens = new List<RefreshToken> { new RefreshToken { Id = Guid.NewGuid().ToString(), Token = tokenResponse.RefreshToken, CreatedDate = DateTime.Now } }
				};

				_hubUsersService.RegisterThirdParty(user);
			}

			return new GoogleUser
			{
				Email = payload.Email,
				Username = payload.Name,
				IdToken = tokenResponse.IdToken,
				AccessToken = tokenResponse.AccessToken,
				RefreshToken = tokenResponse.RefreshToken
			};
		}
	}
}
