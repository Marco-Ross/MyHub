using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Api.Filters
{
	public class LoggedInFilter : IActionFilter
	{
		private readonly IUsersCacheService _usersCacheService;
		public LoggedInFilter(IUsersCacheService usersCacheService)
		{
			_usersCacheService = usersCacheService;
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var hasRefreshCookie = context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken);

			if (hasRefreshCookie && !string.IsNullOrWhiteSpace(refreshToken))
			{
				if (_usersCacheService.IsUserBlacklisted(refreshToken, context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value))
				{
					context.HttpContext.Response.Cookies.Delete(AuthConstants.IdToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.AccessToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.LoggedIn);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.ForgeryToken);

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
				}
			}
		}
	}
}
