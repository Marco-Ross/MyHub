using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyHub.Domain.Background.CleanBackground.Interfaces;

namespace MyHub.Application.BackgroundTasks
{
	public class CleanupBackgroundTaskService : BackgroundService
	{
		private readonly ILogger<CleanupBackgroundTaskService> _logger;
		private readonly ICleanTokensBackgroundService _cleanTokensBackgroundService;

		public CleanupBackgroundTaskService(ILogger<CleanupBackgroundTaskService> logger, ICleanTokensBackgroundService cleanTokensBackgroundService)
		{
			_logger = logger;
			_cleanTokensBackgroundService = cleanTokensBackgroundService;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Starting CleanupBackgroundTaskService");

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation($"CleanupBackgroundTaskService task performing cleanup.");

				try
				{
					_cleanTokensBackgroundService.CleanBackgroundTokens();
				
					await Task.Delay(TimeSpan.FromDays(6), stoppingToken);
				}
				catch (TaskCanceledException exception)
				{
					_logger.LogError(exception, "TaskCanceledException Error", exception.Message);
				}
			}
		}

	}
}
