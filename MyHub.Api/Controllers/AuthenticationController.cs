using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IAuthenticationService _authenticationService;
		private readonly IUserService _userService;
		private readonly IEncryptionService _encryptionService;

		public AuthenticationController(IConfiguration configuration, IAuthenticationService authenticationService, IUserService userService, IEncryptionService encryptionService)
		{
			_configuration = configuration;
			_authenticationService = authenticationService;
			_userService = userService;
			_encryptionService = encryptionService;
		}

		private void SetCookieTokens(Tokens tokens)
		{
			var httpOnlyCookieOptions = new CookieOptions { Domain = _configuration?["Cookies:Domain"], HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append("X-Access-Token", tokens.Token, httpOnlyCookieOptions);
			Response.Cookies.Append("X-Refresh-Token", tokens.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _configuration?["Cookies:Domain"], SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append("X-Logged-In", "true", cookieOptions);
			Response.Cookies.Append("X-Forgery-Token", _encryptionService.Encrypt(_configuration?["Cookies:CsrfToken"]), cookieOptions);
		}

		private void RemoveCookies()
		{
			var cookieDomainOptions = new CookieOptions { Domain = _configuration?["Cookies:Domain"] };

			Response.Cookies.Delete("X-Access-Token", cookieDomainOptions);
			Response.Cookies.Delete("X-Refresh-Token", cookieDomainOptions);
			Response.Cookies.Delete("X-Logged-In", cookieDomainOptions);
			Response.Cookies.Delete("X-Forgery-Token", cookieDomainOptions);
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public IActionResult Post(UserDto userDto)
		{
			var tokens = _authenticationService.AuthenticateUser(userDto.Username, userDto.Password);

			if (tokens is null)
				return Unauthorized("Invalid Login Credentials.");

			SetCookieTokens(tokens);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue("X-Access-Token", out var accessToken) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
				return BadRequest("Access Token or Refresh Token not set.");

			var tokens = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			if (tokens is null)
			{
				RemoveCookies();

				return Forbid("User cannot be authenticated.");
			}

			SetCookieTokens(tokens);

			return Ok();
		}

		[HttpPost("Revoke")]
		public IActionResult Revoke()
		{
			var userId = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var revokedUser = _userService.RevokeUser(userId);

			if(revokedUser is null)
				return Forbid("User does not exist.");

			RemoveCookies();

			return Ok();
		}
	}
}