using MyHub.Domain.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using MyHub.Domain.Users.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

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

			var hasForgeryHeader = context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ForgeryToken, out var forgeryToken);
			var hasForgeryCookie = context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.ForgeryToken, out var forgeryCookie);

			if (hasForgeryHeader || hasForgeryCookie)
			{
				var forgeryTokenDecrypt = _encryptionService?.Decrypt(forgeryToken);
				var forgeryCookieDecrypt = _encryptionService?.Decrypt(forgeryCookie);

				if (forgeryTokenDecrypt != forgeryCookieDecrypt)
				{
					context.HttpContext.Response.Cookies.Delete(AuthConstants.AccessToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.LoggedIn);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.ForgeryToken);

					if (context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken))
						_userService?.RevokeUser(context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty, refreshToken);

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					//signalR logout
				}
			}
		}
	}
}
