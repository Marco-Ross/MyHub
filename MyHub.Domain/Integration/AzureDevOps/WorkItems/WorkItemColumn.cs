using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WorkItems
{
    public class WorkItemColumn
    {
        [JsonPropertyName("referenceName")]
        public string ReferenceName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
