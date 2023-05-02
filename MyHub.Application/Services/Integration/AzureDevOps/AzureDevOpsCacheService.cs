using Microsoft.Extensions.Caching.Memory;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.WorkItems;
using MyHub.Infrastructure.Cache;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MyHub.Application.Services.Integration.AzureDevOps
{
	public class AzureDevOpsCacheService : IAzureDevOpsCacheService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IAzureDevOpsService _azureDevOpsService;
		private readonly MemoryCacheEntryOptions _cacheOptions;

		public AzureDevOpsCacheService(IMemoryCache memoryCache, IAzureDevOpsService azureDevOpsService)
		{
			_memoryCache = memoryCache;
			_azureDevOpsService = azureDevOpsService;
			_cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) };
		}

		private T? GetCache<T>(string key)
		{
			if (_memoryCache.TryGetValue<T>(key, out var cacheValue) && cacheValue is not null)
				return cacheValue;

			return cacheValue;
		}

		private T SetCache<T>(string key, T value)
		{
			return _memoryCache.Set(key, value, _cacheOptions);
		}

		public async Task<WorkItemResults> GetWorkItems()
		{
			var cacheValue = GetCache<WorkItemResults>(CacheKeys.WorkItemResults);

			if (cacheValue is not null)
				return cacheValue;

			var workItemResults = await _azureDevOpsService.GetWorkItems();

			return SetCache(CacheKeys.WorkItemResults, workItemResults);
		}

		public WorkItemResults? UpdateCachedWorkItems(int workItemId, JsonObject updatedWorkItemFields)
		{
			var workItemResults = GetCache<WorkItemResults>(CacheKeys.WorkItemResults);

			if (workItemResults is null || workItemResults.Count == 0 || !workItemResults.WorkItems.Any())
				return null;

			var updatedJsonDict = updatedWorkItemFields.ToDictionary(x => x.Key, y => y.Value);

			var existingWorkItemFields = workItemResults.WorkItems.Find(x => x.Id == workItemId)?.Fields;

			if (existingWorkItemFields is null || updatedJsonDict is null)
				return null;

			foreach (var field in updatedJsonDict)
			{
				var propInfo = existingWorkItemFields.GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == field.Key);

				propInfo?.SetValue(existingWorkItemFields, field.Value?["newValue"]?.ToString());
			}

			return SetCache(CacheKeys.WorkItemResults, workItemResults);
		}
	}
}
