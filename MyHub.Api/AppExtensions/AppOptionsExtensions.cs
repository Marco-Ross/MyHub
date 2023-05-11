using MyHub.Domain.ConfigurationOptions.CorsOriginOptions;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Domain.ConfigurationOptions.Storage;
using MyHub.Domain.ConfigurationOptions;
using MyHub.Domain.ConfigurationOptions.Authentication;

namespace MyHub.Api.AppExtensions
{
	public static class AppOptionsExtensions
	{
		public static void SetAppOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			serviceCollection.Configure<AuthenticationOptions>(configuration.GetSection(ConfigSections.Authentication));
			serviceCollection.Configure<DomainOptions>(configuration.GetSection(ConfigSections.Domain));
			serviceCollection.Configure<CorsOriginOptions>(configuration.GetSection(ConfigSections.CorsOrigin));
			serviceCollection.Configure<StorageOptions>(configuration.GetSection(ConfigSections.Storage));
		}
	}
}
