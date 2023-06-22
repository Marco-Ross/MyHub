using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication.AuthenticationDto;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Authentication;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users.UsersDto;
using MyHub.Domain.Authentication.Facebook;
using MyHub.Domain.ConfigurationOptions.Authentication;
using System.Text.Json;
using MyHub.Domain.Users.Facebook;

namespace MyHub.Api.Controllers.ThirdPartyLogin
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class FacebookAuthController : BaseController
	{
		private readonly IFacebookAuthenticationService _facebookAuthService;
		private readonly AuthenticationOptions _authOptions;
		private readonly ICsrfEncryptionService _encryptionService;

		public FacebookAuthController(IFacebookAuthenticationService facebookAuthService, IOptions<AuthenticationOptions> authOptions, ICsrfEncryptionService encryptionService)
		{
			_authOptions = authOptions.Value;
			_facebookAuthService = facebookAuthService;
			_encryptionService = encryptionService;
		}

		[AllowAnonymous]
		[HttpPost("AccessToken")]
		public async Task<IActionResult> AccessToken(FacebookAccessOptionsDto facebookAccessOptionsDto)
		{
			var facebookUserValidation = await _facebookAuthService.ExchangeAuthCode(facebookAccessOptionsDto.Code);

			if (facebookUserValidation.IsInvalid)
				return BadRequest(facebookUserValidation.ErrorsString);

			SetCookieDetails(facebookUserValidation.ResponseValue);

			return Ok();
		}

		private void SetCookieDetails(FacebookUser facebookUser)
		{
			var hubUserDto = new HubUserDto { Email = facebookUser.Email, Username = facebookUser.Username, LoginIssuer = LoginIssuers.Facebook.Id };

			var httpOnlyCookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.IdToken, facebookUser.IdToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.AccessToken, facebookUser.AccessToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.RefreshToken, facebookUser.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.LoggedIn, JsonSerializer.Serialize(hubUserDto), cookieOptions);
			Response.Cookies.Append(AuthConstants.ForgeryToken, _encryptionService.Encrypt(Guid.NewGuid().ToString()), cookieOptions);
		}
	}
}
