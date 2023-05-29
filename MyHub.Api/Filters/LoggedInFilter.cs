using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.Caching.Memory;
using MyHub.Infrastructure.Cache;

namespace MyHub.Api.Filters
{
	public class LoggedInFilter : IActionFilter
	{
		private readonly IMemoryCache _memoryCache;
		public LoggedInFilter(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var hasRefreshCookie = context.HttpContext.Request.Cookies.TryGetValue(AuthConstants.RefreshToken, out var refreshToken);

			if (hasRefreshCookie && !string.IsNullOrWhiteSpace(refreshToken))
			{
				if (IsBlacklisted(refreshToken, context))
				{
					context.HttpContext.Response.Cookies.Delete(AuthConstants.AccessToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.RefreshToken);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.LoggedIn);
					context.HttpContext.Response.Cookies.Delete(AuthConstants.ForgeryToken);

					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
				}
			}
		}

		private bool IsBlacklisted(string tokenId, ActionExecutingContext context)
		{
			var userId = context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + userId) ?? new List<string>();

			return cacheValue.Contains(tokenId);
		}

		private T? GetCache<T>(string key)
		{
			if (_memoryCache.TryGetValue<T>(key, out var cacheValue) && cacheValue is not null)
				return cacheValue;

			return cacheValue;
		}
	}
}
