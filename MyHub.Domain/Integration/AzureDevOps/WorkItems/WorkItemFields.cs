using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WorkItems
{
    public class WorkItemFields
    {
        [JsonPropertyName("System.Id")]
        public int Id { get; set; }

        [JsonPropertyName("System.Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("System.WorkItemType")]
        public string WorkItemType { get; set; } = string.Empty;

        [JsonPropertyName("System.State")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("System.CreatedDate")]
        public string CreatedDate { get; set; } = string.Empty;

        [JsonPropertyName("System.ChangedDate")]
        public string ChangedDate { get; set; } = string.Empty;

        [JsonPropertyName("System.Description")]
        public string Description { get; set; } = string.Empty;
    }
}
