using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WorkItems
{
    public class WorkItemsQuery
    {
        [JsonPropertyName("queryType")]
        public string QueryType { get; set; } = string.Empty;

        [JsonPropertyName("queryResultType")]
        public string QueryResultType { get; set; } = string.Empty;

        [JsonPropertyName("asOf")]
        public DateTime AsOf { get; set; }

        [JsonPropertyName("columns")]
        public List<WorkItemColumn> Columns { get; set; } = new List<WorkItemColumn>();

        [JsonPropertyName("workItems")]
        public List<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
    }
}
