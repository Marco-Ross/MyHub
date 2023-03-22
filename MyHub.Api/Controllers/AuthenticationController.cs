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
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICsrfEncryptionService _encryptionService;

		public AuthenticationController(IConfiguration configuration, IMapper mapper, IAuthenticationService authenticationService, ICsrfEncryptionService encryptionService)
		{
			_configuration = configuration;
			_authenticationService = authenticationService;
			_encryptionService = encryptionService;
			_mapper	= mapper;
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
		public async Task<IActionResult> Register(RegisterUserDto userDto)
		{
			var registerValidation = await _authenticationService.RegisterUser(_mapper.Map<AccessingUser>(userDto));

			if (registerValidation.IsInvalid)
				return BadRequest(registerValidation.ErrorsString);

			return Ok();
		}
		
		[AllowAnonymous]
		[HttpPost("Register/Complete")]
		public IActionResult CompleteRegisterEmail(RegisterUserCompleteDto registerUserCompleteDto)
		{
			var emailValidation = _authenticationService.VerifyUserEmail(registerUserCompleteDto.UserId, registerUserCompleteDto.RegisterToken);

			if (emailValidation.IsInvalid)
				return BadRequest(emailValidation.ErrorsString);

			return Ok();
		}
		
		[AllowAnonymous]
		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			var passwordResetValidation = await _authenticationService.ResetPasswordEmail(resetPasswordDto.Email);

			if (passwordResetValidation.IsInvalid)
				return BadRequest(passwordResetValidation.ErrorsString);

			return Ok();
		}
		
		[AllowAnonymous]
		[HttpPost("ResetPassword/Complete")]
		public IActionResult CompleteResetPassword(ResetPasswordCompleteDto resetPasswordCompleteDto)
		{
			var passwordResetCompleteValidation = _authenticationService.ResetPassword(resetPasswordCompleteDto.UserId, resetPasswordCompleteDto.Password, resetPasswordCompleteDto.ResetPasswordToken);

			if (passwordResetCompleteValidation.IsInvalid)
				return BadRequest(passwordResetCompleteValidation.ErrorsString);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public IActionResult Login(LoginUserDto userDto)
		{
			var loginValidation = _authenticationService.AuthenticateUser(userDto.Email, userDto.Password);

			if (loginValidation.IsInvalid)
				return BadRequest(loginValidation.ErrorsString);

			SetCookieDetails(loginValidation.ResponseValue);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue("X-Access-Token", out var accessToken) && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)))
				return BadRequest("Access Token or Refresh Token not set");

			var refreshValidation = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			if (refreshValidation.IsInvalid)
			{
				RemoveCookies();

				return Forbid(refreshValidation.ErrorsString);
			}

			SetCookieDetails(refreshValidation.ResponseValue);

			return Ok();
		}

		[HttpPost("Revoke")]
		public IActionResult Revoke()
		{
			var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

			var isUserRevoked= _authenticationService.RevokeUser(userId);

			if(!isUserRevoked)
				return Forbid("User cannot be revoked.");

			RemoveCookies();

			return Ok();
		}
	}
}