using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WebHooks
{
	public class WorkItemEventDto
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("subscriptionId")]
		public string SubscriptionId { get; set; } = string.Empty;

		[JsonPropertyName("notificationId")]
		public int NotificationId { get; set; }

		[JsonPropertyName("eventType")]
		public string EventType { get; set; } = string.Empty;

		[JsonPropertyName("publisherId")]
		public string PublisherId { get; set; } = string.Empty;
	}
}
