using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHub.Domain.Background.CleanBackground.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.BackgroundServices
{
	public class CleanTokensBackgroundService : ICleanTokensBackgroundService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly ILogger<CleanTokensBackgroundService> _logger;

		public CleanTokensBackgroundService(ApplicationDbContext applicationDbContext, ILogger<CleanTokensBackgroundService> logger)
		{
			_applicationDbContext = applicationDbContext;
			_logger = logger;
		}

		public void CleanBackgroundTokens()
		{
			using var transaction = _applicationDbContext.Database.BeginTransaction();

			try
			{
				_applicationDbContext.Database.ExecuteSql($"DELETE FROM RefreshTokens WHERE CreatedDate < DATEADD(month, -6, GETDATE())");

				transaction.Commit();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Refresh Token cleaning failed.");
			}
		}
	}
}
