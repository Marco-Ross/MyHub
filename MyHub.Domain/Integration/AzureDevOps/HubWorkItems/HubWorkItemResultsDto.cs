namespace MyHub.Domain.Integration.AzureDevOps.HubWorkItems
{
	public class HubWorkItemResultsDto
	{
		public int Count { get; set; }
		public List<HubWorkItemDto> WorkItems { get; set; } = new List<HubWorkItemDto>();
	}
}
