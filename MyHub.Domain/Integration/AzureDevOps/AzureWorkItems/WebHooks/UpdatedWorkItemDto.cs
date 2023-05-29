using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.WebHooks
{
    public class UpdatedWorkItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("workItemId")]
        public int WorkItemId { get; set; }

        [JsonPropertyName("fields")]
        public JsonObject UpdatedWorkItemFields { get; set; } = new JsonObject();
    }
}
