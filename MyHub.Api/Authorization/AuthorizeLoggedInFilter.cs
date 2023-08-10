using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;

namespace MyHub.Api.Authorization
{
	public class AuthorizeLoggedInFilter : IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.LoggedIn, out var loggedIn))
			{
				if (!string.IsNullOrWhiteSpace(loggedIn) && context.HttpContext.User.Identity is not null && context.HttpContext.User.Identity.IsAuthenticated)
					return;

				context.Result = new UnauthorizedObjectResult("User not authenticated.");
				return;
			}
		}
	}
}
