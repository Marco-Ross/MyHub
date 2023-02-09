using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Application.Services.Users;
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

		[AllowAnonymous]
		[HttpPost("Login")]
		public IActionResult Post(UserDto userDto)
		{
			var tokens = _authenticationService.AuthenticateUser(userDto.Username, userDto.Password);

			Response.Cookies.Append("X-Access-Token", tokens.Token, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });
			Response.Cookies.Append("X-Refresh-Token", tokens.RefreshToken, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true }); // expires?

			return Ok();
		}

		[AllowAnonymous]
		[HttpGet("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue("X-Access-Token", out var accessToken) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
				return BadRequest();

			var newAccessTokens = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			Response.Cookies.Append("X-Access-Token", newAccessTokens.Token, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });
			Response.Cookies.Append("X-Refresh-Token", newAccessTokens.RefreshToken, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });

			return Ok();
		}

		[HttpPost]
		[Route("Revoke")]
		public IActionResult Revoke()
		{
			var userId = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var user = _userService.GetUser(userId);

			if (user == null) 
				return BadRequest();

			user.RefreshToken = string.Empty;

			_userService.UpdateUser(user);

			Response.Cookies.Delete("X-Access-Token");
			Response.Cookies.Delete("X-Refresh-Token");

			return NoContent();
		}

		[HttpGet]
		[Route("Test")]
		public IActionResult Test()
		{
			return Ok(new {Number =  "This is a test" + new Random().Next() });
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("SuperTest")]
		public IActionResult SuperTest()
		{
			return Ok(new { Number = "This is a test" + new Random().Next() });
		}
	}
}