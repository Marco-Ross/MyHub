using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication;
using MyHub.Domain.ConfigurationOptions.Authentication;

namespace MyHub.Api.Filters
{
	public class ApiKeyAuthFilter : IAuthorizationFilter
	{
		private readonly AuthenticationOptions _authOptions;
		public ApiKeyAuthFilter(IOptions<AuthenticationOptions> authOptions)
		{
			_authOptions = authOptions.Value;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if(!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeader, out var extractedApiKey))
			{
				context.Result = new UnauthorizedObjectResult("Api Key missing from header.");
				return;
			}

			if (!_authOptions.ApiKey.Equals(extractedApiKey))
			{
				context.Result = new UnauthorizedObjectResult("Invalid Api Key.");
				return;
			}
		}
	}
}
