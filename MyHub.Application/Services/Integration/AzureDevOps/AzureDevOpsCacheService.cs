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

		public async Task<WorkItemResults> GetWorkItems()
		{
			if (_memoryCache.TryGetValue<WorkItemResults>(CacheKeys.WorkItemResults, out var cacheValue) && cacheValue is not null && cacheValue.Count != 0)
				return cacheValue ?? new WorkItemResults();

			var workItemResults = await _azureDevOpsService.GetWorkItems();

			return SetCache(CacheKeys.WorkItemResults, workItemResults);
		}

		public async Task<WorkItemResults> UpdateCache(int workItemId, JsonObject updatedWorkItemFields)
		{
			var workItemResults = await GetWorkItems();

			if (workItemResults is null || workItemResults.Count == 0 || !workItemResults.WorkItems.Any())
				return new WorkItemResults();

			var updatedJsonDict = updatedWorkItemFields.ToDictionary(x => x.Key, y => y.Value);

			var existingWorkItemFields = workItemResults.WorkItems.Find(x => x.Id == workItemId)?.Fields;

			if (existingWorkItemFields is null || updatedJsonDict is null)
				return new WorkItemResults();

			foreach (var field in updatedJsonDict)
			{
				var propInfo = existingWorkItemFields.GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == field.Key);

				propInfo?.SetValue(existingWorkItemFields, field.Value?["newValue"]?.ToString());
			}

			return SetCache(CacheKeys.WorkItemResults, workItemResults);
		}

		private T SetCache<T>(string key, T value)
		{
			return _memoryCache.Set(key, value, _cacheOptions);
		}
	}
}
