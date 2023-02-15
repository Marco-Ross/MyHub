using AutoMapper;
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
		private readonly IMapper _mapper;
		private readonly IAuthenticationService _authenticationService;
		private readonly IUserService _userService;

		public AuthenticationController(IMapper mapper, IAuthenticationService authenticationService, IUserService userService)
		{
			_authenticationService = authenticationService;
			_userService = userService;
			_mapper = mapper;
		}

		private void SetCookieTokens(Tokens tokens)
		{
			Response.Cookies.Append("X-Access-Token", tokens.Token, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue });
			Response.Cookies.Append("X-Refresh-Token", tokens.RefreshToken, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue });
			Response.Cookies.Append("X-Logged-In", "true", new CookieOptions { SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue });
		}

		private void RemoveCookies()
		{
			Response.Cookies.Delete("X-Access-Token");
			Response.Cookies.Delete("X-Refresh-Token");
			Response.Cookies.Delete("X-Logged-In");
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
		[HttpGet("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue("X-Access-Token", out var accessToken) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
				return BadRequest("Access Token or Refresh Token not set.");

			var tokens = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			if (tokens is null)
			{
				RemoveCookies();

				return Unauthorized("User cannot be authenticated.");
			}

			SetCookieTokens(tokens);

			return Ok();
		}

		[HttpGet]
		[Route("Revoke")]
		public IActionResult Revoke()
		{
			var userId = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			_userService.RevokeUser(userId);

			RemoveCookies();

			return Ok();
		}

		[HttpGet]
		[Route("Test")]
		public IActionResult Test()
		{
			return Ok(new { Number = "This is a test" + new Random().Next() });
		}

		[HttpGet]
		[Route("SuperTest")]
		public IActionResult SuperTest()
		{
			return Ok(new { Number = "This is a test" + new Random().Next() });
		}
	}
}