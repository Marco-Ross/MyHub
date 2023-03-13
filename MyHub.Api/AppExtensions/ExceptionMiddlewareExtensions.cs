using Microsoft.AspNetCore.Diagnostics;
using MyHub.Domain.Exceptions;
using System.Net;

namespace MyHub.Api.AppExtensions
{
	public static class ExceptionMiddlewareExtensions
	{
		public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger<ExceptionDetails> logger)
		{
			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";
					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						logger.LogError($"Exception thrown: {contextFeature.Error}");
						await context.Response.WriteAsync(new ExceptionDetails()
						{
							StatusCode = context.Response.StatusCode,
							Message = contextFeature.Error.Message,
							InnerMessage = contextFeature.Error.InnerException?.Message
						}.ToString());
					}
				});
			});

		}
	}
}
