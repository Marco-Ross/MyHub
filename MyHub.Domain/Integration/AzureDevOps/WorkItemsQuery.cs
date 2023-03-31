using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps
{
	public class WorkItemsQuery
	{
		[JsonProperty("queryType")]
		public string QueryType { get; set; } = string.Empty;

		[JsonProperty("queryResultType")]
		public string QueryResultType { get; set; } = string.Empty;
		
		[JsonProperty("asOf")]
		public DateTime AsOf { get; set; }

		[JsonProperty("columns")]
		public List<WorkItemColumn> Columns { get; set; } = new List<WorkItemColumn>();
		
		[JsonProperty("workItems")]
		public List<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
	}
}
