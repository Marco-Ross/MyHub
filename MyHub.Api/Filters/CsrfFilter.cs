using MyHub.Domain.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Authentication;

namespace MyHub.Application.Services.Authentication
{
	public class CsrfFilter : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var requestScope = context.HttpContext.RequestServices;

			var _encryptionService = requestScope.GetService(typeof(ICsrfEncryptionService)) as ICsrfEncryptionService;
			var _configuration = requestScope.GetService(typeof(IOptions<AuthenticationOptions>)) as IOptions<AuthenticationOptions>;

			var hasForgeryCookie = context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.ForgeryTokenHeader, out var forgeryToken);

			if (hasForgeryCookie)
			{
				var forgeryTokenDecrypt = _encryptionService?.Decrypt(forgeryToken);

				if (forgeryTokenDecrypt != _configuration?.Value.Cookies.CsrfToken)
				{
					context.HttpContext.Response.Cookies.Delete(AuthConstants.AccessTokenHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshTokenHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.LoggedInHeader);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.ForgeryTokenHeader);

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					//signalR logout
				}
			}
		}
	}
}
