using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.WorkItems
{
    public class WorkItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("rev")]
        public int Rev { get; set; }

        [JsonPropertyName("fields")]
        public WorkItemFields? Fields { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
