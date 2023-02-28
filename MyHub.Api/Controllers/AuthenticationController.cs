using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.UsersDto;
using Newtonsoft.Json;

namespace MyHub.Controllers
{
    [Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICsrfEncryptionService _encryptionService;

		public AuthenticationController(IMapper mapper, IConfiguration configuration, IAuthenticationService authenticationService, ICsrfEncryptionService encryptionService)
		{
			_mapper = mapper;
			_configuration = configuration;
			_authenticationService = authenticationService;
			_encryptionService = encryptionService;
		}

		private void SetCookieDetails(LoginDetails loginTokens)
		{
			var httpOnlyCookieOptions = new CookieOptions { Domain = _configuration?["Cookies:Domain"], HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append("X-Access-Token", loginTokens.Tokens.Token, httpOnlyCookieOptions);
			Response.Cookies.Append("X-Refresh-Token", loginTokens.Tokens.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _configuration?["Cookies:Domain"], SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append("X-Logged-In", JsonConvert.SerializeObject(loginTokens.HubUserDto), cookieOptions);
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
		[HttpPost("Register")]
		public IActionResult Register(RegisterUserDto userDto)
		{
			var isRegisterSuccessful = _authenticationService.RegisterUser(_mapper.Map<User>(userDto));

			if (!isRegisterSuccessful)
				return Unauthorized("Email already exists");

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public IActionResult Login(LoginUserDto userDto)
		{
			var loginDetails = _authenticationService.AuthenticateUser(userDto.Email, userDto.Password);

			if (loginDetails is null)
				return Unauthorized("Invalid Login Credentials");

			SetCookieDetails(loginDetails);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue("X-Access-Token", out var accessToken) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
				return BadRequest("Access Token or Refresh Token not set");

			var loginDetails = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			if (loginDetails is null)
			{
				RemoveCookies();

				return Forbid("User cannot be authenticated");
			}

			SetCookieDetails(loginDetails);

			return Ok();
		}

		[HttpPost("Revoke")]
		public IActionResult Revoke()
		{
			var userId = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var isUserRevoked= _authenticationService.RevokeUser(userId);

			if(!isUserRevoked)
				return Forbid("User does not exist");

			RemoveCookies();

			return Ok();
		}
	}
}