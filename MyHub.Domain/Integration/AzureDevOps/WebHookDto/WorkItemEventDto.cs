using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps.WebHookDto
{
	public class WorkItemEventDto
	{
		[JsonProperty("id")]
		public string Id { get; set; } = string.Empty;

		[JsonProperty("subscriptionId")]
		public string SubscriptionId { get; set; } = string.Empty;
		
		[JsonProperty("notificationId")]
		public int NotificationId { get; set; }

		[JsonProperty("eventType")]
		public string EventType { get; set; } = string.Empty;

		[JsonProperty("publisherId")]
		public string PublisherId { get; set; } = string.Empty;
	}
}
