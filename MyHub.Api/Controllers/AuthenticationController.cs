using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Api.Controllers;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Users;
using MyHub.Domain.Users.UsersDto;
using Newtonsoft.Json;

namespace MyHub.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : BaseController
	{
		private readonly AuthenticationOptions _authOptions;
		private readonly IMapper _mapper;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICsrfEncryptionService _encryptionService;

		public AuthenticationController(IOptions<AuthenticationOptions> authOptions, IMapper mapper, IAuthenticationService authenticationService, ICsrfEncryptionService encryptionService)
		{
			_authOptions = authOptions.Value;
			_authenticationService = authenticationService;
			_encryptionService = encryptionService;
			_mapper	= mapper;
		}

		private void SetCookieDetails(LoginDetails loginTokens)
		{
			var httpOnlyCookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.AccessTokenHeader, loginTokens.Tokens.AccessToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.RefreshTokenHeader, loginTokens.Tokens.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.LoggedInHeader, JsonConvert.SerializeObject(loginTokens.HubUserDto), cookieOptions);
			Response.Cookies.Append(AuthConstants.ForgeryTokenHeader, _encryptionService.Encrypt(_authOptions.Cookies.CsrfToken), cookieOptions);
		}

		private void RemoveCookies()
		{
			var cookieDomainOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain };

			Response.Cookies.Delete(AuthConstants.AccessTokenHeader, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.RefreshTokenHeader, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.LoggedInHeader, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.ForgeryTokenHeader, cookieDomainOptions);
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
			if (!(Request.Cookies.TryGetValue(AuthConstants.AccessTokenHeader, out var accessToken) && Request.Cookies.TryGetValue(AuthConstants.RefreshTokenHeader, out var refreshToken)))
				return BadRequest("Access Token or Refresh Token not set");

			var refreshValidation = _authenticationService.RefreshUserAuthentication(accessToken, refreshToken);

			if (refreshValidation.IsInvalid)
			{
				RemoveCookies();

				return Forbid(JwtBearerDefaults.AuthenticationScheme);
			}

			SetCookieDetails(refreshValidation.ResponseValue);

			return Ok();
		}

		[HttpPost("Revoke")]
		public IActionResult Revoke()
		{
			if(!Request.Cookies.TryGetValue(AuthConstants.RefreshTokenHeader, out var refreshToken))
				return BadRequest("Refresh Token not set");

			var isUserRevoked= _authenticationService.RevokeUser(UserId, refreshToken);

			if(!isUserRevoked)
				return Forbid(JwtBearerDefaults.AuthenticationScheme);

			RemoveCookies();

			return Ok();
		}
	}
}