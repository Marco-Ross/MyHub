using MyHub.Domain.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

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

				if (forgeryTokenDecrypt != _configuration?["CSRF:Token"])
					throw new Exception("CSRF detected.");
			}
		}
	}
}
