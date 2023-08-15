using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication.AuthenticationDto;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Authentication;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users.UsersDto;
using MyHub.Domain.ConfigurationOptions.Authentication;
using System.Text.Json;
using MyHub.Domain.Authentication.Github;
using MyHub.Domain.Users.Github;

namespace MyHub.Api.Controllers.ThirdPartyLogin
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class GithubAuthController : BaseController
	{
		private readonly IGithubAuthenticationService _githubAuthService;
		private readonly AuthenticationOptions _authOptions;
		private readonly ICsrfEncryptionService _encryptionService;

		public GithubAuthController(IGithubAuthenticationService githubAuthService, IOptions<AuthenticationOptions> authOptions, ICsrfEncryptionService encryptionService)
		{
			_authOptions = authOptions.Value;
			_githubAuthService = githubAuthService;
			_encryptionService = encryptionService;
		}

		[AllowAnonymous]
		[HttpPost("AccessToken")]
		public async Task<IActionResult> AccessToken(GithubAccessOptionsDto githubAccessOptionsDto)
		{
			var githubUserValidation = await _githubAuthService.ExchangeAuthCode(githubAccessOptionsDto.Code);

			if (githubUserValidation.IsInvalid)
				return BadRequest(githubUserValidation.ErrorsString);

			SetCookieDetails(githubUserValidation.ResponseValue);

			return Ok();
		}

		private void SetCookieDetails(GithubUser githubUser)
		{
			var hubUserDto = new HubUserDto { Email = githubUser.Email, Username = githubUser.Username + "test", LoginIssuer = LoginIssuers.Github.Id };

			var httpOnlyCookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.IdToken, githubUser.IdToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.AccessToken, githubUser.AccessToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.RefreshToken, githubUser.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.LoggedIn, JsonSerializer.Serialize(hubUserDto), cookieOptions);
			Response.Cookies.Append(AuthConstants.ForgeryToken, _encryptionService.Encrypt(Guid.NewGuid().ToString()), cookieOptions);
		}
	}
}
