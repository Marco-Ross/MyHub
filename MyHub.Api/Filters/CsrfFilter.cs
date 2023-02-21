using MyHub.Domain.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MyHub.Application.Services.Authentication
{
	public class CsrfFilter : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var requestScope = context.HttpContext.RequestServices;

			var _encryptionService = requestScope.GetService(typeof(IEncryptionService)) as IEncryptionService;
			var _configuration = requestScope.GetService(typeof(IConfiguration)) as IConfiguration;

			var hasForgeryCookie = context.HttpContext.Request.Cookies.TryGetValue("X-Forgery-Token", out var forgeryToken);

			if (hasForgeryCookie)
			{
				var forgeryTokenDecrypt = _encryptionService?.Decrypt(forgeryToken);

				if (forgeryTokenDecrypt != _configuration?["Cookies:CsrfToken"])
				{
					context.HttpContext.Response.Cookies.Delete("X-Access-Token");
					context.HttpContext.Response.Cookies.Delete("X-Refresh-Token");
					context.HttpContext.Response.Cookies.Delete("X-Logged-In");
					context.HttpContext.Response.Cookies.Delete("X-Forgery-Token");

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					//signalR logout
				}
			}
		}
	}
}
