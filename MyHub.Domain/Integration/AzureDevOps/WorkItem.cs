using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps
{
	public class WorkItem
	{
		[JsonProperty("id")]
		public string Id { get; set; } = string.Empty;
		
		[JsonProperty("rev")]
		public int Rev { get; set; }

		[JsonProperty("fields")]
		public WorkItemFields? Fields { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; } = string.Empty;
	}
}
