using MyHub.Domain.Integration.AzureDevOps;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using System.Net.Http.Json;

namespace MyHub.Application.Services.Integration.AzureDevOps
{
	public class AzureDevOpsService : IAzureDevOpsService
	{
		private readonly HttpClient _httpClient;

		public AzureDevOpsService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<WorkItemResults> GetWorkItems()
		{
			var workItemsQuery = await GetWorkItemsQuery();

			if (workItemsQuery is null || !workItemsQuery.WorkItems.Any())
				return await Task.FromResult(new WorkItemResults());

			var json = JsonContent.Create(new { ids = workItemsQuery.WorkItems.Select(x => x.Id), fields = new List<string> { "System.Id", "System.Title", "System.WorkItemType", "System.State", "System.CreatedDate", "System.ChangedDate", "System.Description" } });
			var workItemsBatchRequest = new HttpRequestMessage(HttpMethod.Post, "wit/workitemsbatch?api-version=7.0")
			{
				Content = json
			};

			var response = await _httpClient.SendAsync(workItemsBatchRequest);

			var workItemResults = await response.Content.ReadAsAsync<WorkItemResults>();

			if(workItemResults is null || !workItemResults.WorkItems.Any())
				return await Task.FromResult(new WorkItemResults());

			return workItemResults;
		}

		private async Task<WorkItemsQuery> GetWorkItemsQuery()
		{
			var idRequest = new HttpRequestMessage(HttpMethod.Post, "wit/wiql?api-version=7.0")
			{
				Content = JsonContent.Create(new { query = "Select [System.Id] From WorkItems" })
			};
			var response = await _httpClient.SendAsync(idRequest);

			return await response.Content.ReadAsAsync<WorkItemsQuery>();
		}
	}
}
