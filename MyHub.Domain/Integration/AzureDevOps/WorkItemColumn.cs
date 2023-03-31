using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps
{
	public class WorkItemColumn
	{
		[JsonProperty("referenceName")]
		public string ReferenceName { get; set; } = string.Empty;

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("url")]
		public string Url { get; set; } = string.Empty;
	}
}
