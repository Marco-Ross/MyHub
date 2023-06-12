using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.AuthenticationDto;
using MyHub.Domain.Authentication.Google;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users.UsersDto;
using System.Text.Json;

namespace MyHub.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class GoogleAuthController : BaseController
	{
		private readonly IGoogleAuthenticationService _googleAuthService;
		private readonly AuthenticationOptions _authOptions;
		private readonly ICsrfEncryptionService _encryptionService;

		public GoogleAuthController(IGoogleAuthenticationService googleAuthService, IOptions<AuthenticationOptions> authOptions, ICsrfEncryptionService encryptionService)
		{
			_authOptions = authOptions.Value;
			_googleAuthService = googleAuthService;
			_encryptionService = encryptionService;
		}

		[AllowAnonymous]
		[HttpPost("AccessToken")]
		public async Task<IActionResult> AccessToken(GoogleAccessOptionsDto googleAccessOptionsDto)
		{
			var googleUser = await _googleAuthService.ExchangeAuthCode(googleAccessOptionsDto.AuthUser, googleAccessOptionsDto.Code);

			SetCookieDetails(googleUser);

			return Ok();
		}

		private void SetCookieDetails(GoogleUser googleUser)
		{
			var hubUserDto = new HubUserDto { Email = googleUser.Email, Username = googleUser.Username, LoginIssuer = LoginIssuers.Google.Name };

			var httpOnlyCookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.IdToken, googleUser.IdToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.AccessToken, googleUser.AccessToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.RefreshToken, googleUser.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.LoggedIn, JsonSerializer.Serialize(hubUserDto), cookieOptions);
			Response.Cookies.Append(AuthConstants.ForgeryToken, _encryptionService.Encrypt(Guid.NewGuid().ToString()), cookieOptions);
		}
	}
}
