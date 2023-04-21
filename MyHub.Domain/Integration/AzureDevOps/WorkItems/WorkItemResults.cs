using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WorkItems
{
    public class WorkItemResults
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
    }
}
