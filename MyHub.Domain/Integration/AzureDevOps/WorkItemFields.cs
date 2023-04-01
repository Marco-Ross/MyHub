using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps
{
	public class WorkItemFields
	{
		[JsonProperty("System.Id")]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("System.Title")]
		public string Title { get; set; } = string.Empty;

		[JsonProperty("System.WorkItemType")]
		public string WorkItemType { get; set; } = string.Empty;

		[JsonProperty("System.State")]
		public string State { get; set; } = string.Empty;

		[JsonProperty("System.CreatedDate")]
		public string CreatedDate { get; set; } = string.Empty;
		
		[JsonProperty("System.ChangedDate")]
		public string ChangedDate { get; set; } = string.Empty;

		[JsonProperty("System.Description")]
		public string Description { get; set; } = string.Empty;
	}
}
