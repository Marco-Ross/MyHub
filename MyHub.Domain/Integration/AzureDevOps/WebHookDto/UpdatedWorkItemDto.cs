using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps.WebHookDto
{
	public class UpdatedWorkItemDto
	{
		[JsonProperty("id")]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("workItemId")]
		public string WorkItemId { get; set; } = string.Empty;

		[JsonProperty("fields")]
		public WorkItemFields UpdatedWorkItemFields { get; set; } = new WorkItemFields();
	}
}
