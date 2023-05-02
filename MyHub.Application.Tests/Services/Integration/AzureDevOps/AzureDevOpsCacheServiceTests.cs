using Microsoft.Extensions.Caching.Memory;
using MyHub.Application.Services.Integration.AzureDevOps;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.WorkItems;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MyHub.Application.Tests.Services.Integration.AzureDevOps
{
	public class AzureDevOpsCacheServiceTests : ApplicationTestBase
	{
		private readonly IAzureDevOpsCacheService _azureDevOpsCacheService;
		private readonly Mock<IMemoryCache> _memoryCache = new();
		private readonly Mock<IAzureDevOpsService> _azureDevOpsService = new();

		public AzureDevOpsCacheServiceTests()
		{
			_azureDevOpsCacheService = new AzureDevOpsCacheService(_memoryCache.Object, _azureDevOpsService.Object);
		}

		[Fact]
		public async Task GetWorkItems_HasCache_ReturnsCache()
		{
			//Arrange
			var expectedCacheValue = (object)new WorkItemResults { Count = 1, WorkItems = new List<WorkItem> { new WorkItem { Id = 1 } } };
			_memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedCacheValue)).Returns(true);

			//Act
			var workItems = await _azureDevOpsCacheService.GetWorkItems();

			//Assert
			Assert.Equal(workItems, expectedCacheValue);
		}

		[Fact]
		public async Task GetWorkItems_HasNoCache_SetsCacheReturnsWorkItems()
		{
			//Arrange
			var expectedWorkItems = new WorkItemResults { Count = 1, WorkItems = new List<WorkItem> { new WorkItem { Id = 1, Fields = new WorkItemFields() } } };
			_azureDevOpsService.Setup(x => x.GetWorkItems()).Returns(async Task<WorkItemResults> () => await Task.FromResult(expectedWorkItems));
			_memoryCache.Setup(x => x.CreateEntry(It.IsAny<string>())).Returns(Mock.Of<ICacheEntry>);

			//Act
			var workItems = await _azureDevOpsCacheService.GetWorkItems();

			//Assert
			Assert.NotEmpty(workItems.WorkItems);
			_memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()));
		}

		[Fact]
		public void UpdateCache_NoCachedWorkItems_SetCacheNotCalled()
		{
			//Arrange
			var updatedWorkItemFields = JsonNode.Parse(JsonSerializer.Serialize(new WorkItemFields()))!.AsObject();

			//Act
			_azureDevOpsCacheService.UpdateCachedWorkItems(1, updatedWorkItemFields);

			//Assert
			_memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
		}

		[Fact]
		public void UpdateCache_WorkItemsEmpty_SetCacheNotCalled()
		{
			//Arrange
			var expectedCacheValue = (object)new WorkItemResults { Count = 0, WorkItems = new List<WorkItem> { new WorkItem { Id = 1 } } };
			_memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedCacheValue)).Returns(true);

			var updatedWorkItemFields = JsonNode.Parse(JsonSerializer.Serialize(new WorkItemFields()))!.AsObject();

			//Act
			_azureDevOpsCacheService.UpdateCachedWorkItems(1, updatedWorkItemFields);

			//Assert
			_memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
		}

		[Fact]
		public void UpdateCache_NoMatchingWorkItemIdFound_SetCacheNotCalled()
		{
			//Arrange
			var expectedCacheValue = (object)new WorkItemResults { Count = 1, WorkItems = new List<WorkItem> { new WorkItem() } };
			_memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedCacheValue)).Returns(true);

			var updatedWorkItemFields = JsonNode.Parse(JsonSerializer.Serialize(new WorkItemFields()))!.AsObject();

			//Act
			_azureDevOpsCacheService.UpdateCachedWorkItems(1, updatedWorkItemFields);

			//Assert
			_memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
		}

		[Fact]
		public void UpdateCache_MatchingWorkItemIdFound_CacheUpdated()
		{
			//Arrange
			var expectedCacheValue = (object)new WorkItemResults { Count = 1, WorkItems = new List<WorkItem> { new WorkItem { Id = 1, Fields = new WorkItemFields { State = "To Do" } } } };
			_memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedCacheValue)).Returns(true);
			_memoryCache.Setup(x => x.CreateEntry(It.IsAny<string>())).Returns(Mock.Of<ICacheEntry>);

			var updatedWorkItemFields = JsonNode.Parse("{\"System.State\":{\"oldValue\":\"To Do\", \"newValue\": \"Done\"} }")!.AsObject();

			//Act
			var updatedCache = _azureDevOpsCacheService.UpdateCachedWorkItems(1, updatedWorkItemFields);

			//Assert
			Assert.Equal("Done", updatedCache?.WorkItems.First()?.Fields?.State);
			_memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);
		}
	}
}
