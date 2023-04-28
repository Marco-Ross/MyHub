using MyHub.Application.BackgroundTasks;

namespace MyHub.Api.AppExtensions
{
	public static class AddBackgroundServiceExtensions
	{
		public static void AddBackgroundServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddHostedService<CleanupBackgroundTaskService>();
		}
	}
}
