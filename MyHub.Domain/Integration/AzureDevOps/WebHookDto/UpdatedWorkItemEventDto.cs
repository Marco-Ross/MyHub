using Newtonsoft.Json;

namespace MyHub.Domain.Integration.AzureDevOps.WebHookDto
{
	public class UpdatedWorkItemEventDto : WorkItemEventDto
	{
		[JsonProperty("resource")]
		public UpdatedWorkItemDto UpdatedWorkItemDto { get; set; } = new UpdatedWorkItemDto();
	}
}
