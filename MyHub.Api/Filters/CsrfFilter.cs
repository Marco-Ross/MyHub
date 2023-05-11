using MyHub.Domain.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Users.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Azure.Core;

namespace MyHub.Application.Services.Authentication
{
	public class CsrfFilter : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var requestScope = context.HttpContext.RequestServices;

			var _encryptionService = requestScope.GetService(typeof(ICsrfEncryptionService)) as ICsrfEncryptionService;
			var _userService = requestScope.GetService(typeof(IUserService)) as IUserService;
			var _configuration = requestScope.GetService(typeof(IOptions<AuthenticationOptions>)) as IOptions<AuthenticationOptions>;

			var hasForgeryCookie = context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.ForgeryTokenHeader, out var forgeryToken);

			if (hasForgeryCookie)
			{
				var forgeryTokenDecrypt = forgeryToken;//_encryptionService?.Decrypt();

				if (forgeryTokenDecrypt != _configuration?.Value.Cookies.CsrfToken)
				{
					context.HttpContext.Response.Cookies.Delete(AuthConstants.AccessTokenHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshTokenHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.LoggedInHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.ForgeryTokenHeader);

					if (context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshTokenHeader, out var refreshToken))
					{
						_userService?.RevokeUser(context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty, refreshToken);
					}

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					//signalR logout
				}
			}
		}
	}
}
