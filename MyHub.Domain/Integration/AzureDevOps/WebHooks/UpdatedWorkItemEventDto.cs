using System.Text.Json.Serialization;

namespace MyHub.Domain.Integration.AzureDevOps.WebHooks
{
	public class UpdatedWorkItemEventDto : WorkItemEventDto
	{
		[JsonPropertyName("resource")]
		public UpdatedWorkItemDto UpdatedWorkItemDto { get; set; } = new UpdatedWorkItemDto();
	}
}
