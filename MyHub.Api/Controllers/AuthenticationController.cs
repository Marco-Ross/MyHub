using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyHub.Api.Controllers;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Google;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Users;
using MyHub.Domain.Users.UsersDto;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ISharedAuthServiceFactory _sharedAuthServiceFactory;

		public AuthenticationController(IOptions<AuthenticationOptions> authOptions, IMapper mapper, IAuthenticationService authenticationService,
			ICsrfEncryptionService encryptionService, ILogger<AuthenticationController> logger, ISharedAuthServiceFactory sharedAuthServiceFactory)
		{
			_authOptions = authOptions.Value;
			_authenticationService = authenticationService;
			_encryptionService = encryptionService;
			_mapper = mapper;
			_logger = logger;
			_sharedAuthServiceFactory = sharedAuthServiceFactory;
		}

		private void SetCookieDetails(LoginDetails loginTokens)
		{
			var httpOnlyCookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.IdToken, loginTokens.Tokens.IdToken, httpOnlyCookieOptions);
			Response.Cookies.Append(AuthConstants.RefreshToken, loginTokens.Tokens.RefreshToken, httpOnlyCookieOptions);

			var cookieOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain, SameSite = SameSiteMode.Strict, Secure = true, Expires = DateTime.MaxValue };
			Response.Cookies.Append(AuthConstants.LoggedIn, JsonSerializer.Serialize(loginTokens.HubUserDto), cookieOptions);
			Response.Cookies.Append(AuthConstants.ForgeryToken, _encryptionService.Encrypt(Guid.NewGuid().ToString()), cookieOptions);
		}

		private void RemoveCookies()
		{
			var cookieDomainOptions = new CookieOptions { Domain = _authOptions.Cookies.Domain };

			Response.Cookies.Delete(AuthConstants.IdToken, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.AccessToken, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.RefreshToken, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.LoggedIn, cookieDomainOptions);
			Response.Cookies.Delete(AuthConstants.ForgeryToken, cookieDomainOptions);
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

		[HttpPost("ResetPasswordLoggedIn")]
		public IActionResult ResetPasswordLoggedIn(ResetPasswordLoggedInDto resetPasswordDto)
		{
			if (!Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken))
				return BadRequest("Refresh Token not set");

			var passwordResetValidation = _authenticationService.ResetPasswordLoggedIn(UserId, resetPasswordDto.OldPassword, resetPasswordDto.Password, refreshToken);

			if (passwordResetValidation.IsInvalid)
				return BadRequest(passwordResetValidation.ErrorsString);

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
		
		[HttpPost("LoginToContinue")]
		public IActionResult LoginToContinue(LoginUserDto userDto)
		{
			var idToken = _authenticationService.AuthenticateUserGetTokens(UserId, userDto.Email, userDto.Password);

			if (string.IsNullOrWhiteSpace(idToken))
				return BadRequest("Invalid Login");

			return Ok(new { idToken });
		}

		[AllowAnonymous]
		[HttpPost("Refresh")]
		public IActionResult Refresh()
		{
			if (!(Request.Cookies.TryGetValue(AuthConstants.IdToken, out var idToken) && Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken)))
			{
				RemoveCookies();
				return BadRequest("Access Token or Refresh Token not set");
			}

			var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

			var refreshValidation = _sharedAuthServiceFactory.GetAuthService(jwtToken.Issuer).RefreshUserAuthentication(idToken, refreshToken);

			if (refreshValidation.IsInvalid)
			{
				_logger.LogError("Failed to refresh token with error: {RefreshError}", refreshValidation.ErrorsString);

				RemoveCookies();

				return Forbid(JwtBearerDefaults.AuthenticationScheme);
			}

			SetCookieDetails(refreshValidation.ResponseValue);

			return Ok();
		}

		[HttpPost("Revoke")]
		public IActionResult Revoke()
		{
			if (!Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken))
				return BadRequest("Refresh Token not set");

			var isUserRevoked = _authenticationService.RevokeUser(UserId, refreshToken);

			if (!isUserRevoked)
				return Forbid(JwtBearerDefaults.AuthenticationScheme);

			RemoveCookies();

			return Ok();
		}

		[HttpDelete()]
		public async Task<IActionResult> DeleteUser()
		{
			await _authenticationService.DeleteUser(UserId);

			RemoveCookies();

			return Ok();
		}

		[HttpPost("ChangeEmail")]
		public async Task<IActionResult> ChangeEmail(ChangeEmailDto changeEmailDto)
		{
			var changeEmailValidator = await _authenticationService.ChangeUserEmail(UserId, changeEmailDto.Email, changeEmailDto.IdToken);

			if (changeEmailValidator.IsInvalid)
				return BadRequest(changeEmailValidator.ErrorsString);

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("ChangeEmail/Complete")]
		public IActionResult ChangeEmailComplete(ChangeEmailCompleteDto changeEmailCompleteDto)
		{
			var changeEmailCompleteValidation = _authenticationService.ChangeUserEmailComplete(changeEmailCompleteDto.UserId, changeEmailCompleteDto.ChangeEmailToken);

			if (changeEmailCompleteValidation.IsInvalid)
				return BadRequest(changeEmailCompleteValidation.ErrorsString);

			RemoveCookies();

			return Ok();
		}
	}
}