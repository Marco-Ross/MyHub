using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps
{
	public class WorkItemResults
	{
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("value")]
		public List<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
	}
}
