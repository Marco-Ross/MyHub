using MyHub.Application.Services.Integration.AzureDevOps;
using MyHub.Application.Services.Users;
using MyHub.Domain.ConfigurationOptions;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.Interfaces;
using MyHub.Domain.Users.Google;
using System.Net.Http.Headers;
using System.Text;

namespace MyHub.Api.AppExtensions
{
    public static class HttpClientExtensions
	{
		public static void ConfigureHttpClients(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			serviceCollection.AddHttpClient<IAzureDevOpsService, AzureDevOpsService>(c =>
			{
				c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "",
					configuration.GetSection(ConfigSections.Authentication).Get<AuthenticationOptions>()?.PAT.AzureDevOps ?? string.Empty))));
				c.BaseAddress = new Uri("https://dev.azure.com/marcoshub/MyHub/_apis/");
			});
			
			serviceCollection.AddHttpClient<IGoogleUsersService, GoogleUsersService>(c =>
			{
				c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));
			});
		}
	}
}
