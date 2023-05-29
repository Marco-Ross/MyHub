namespace MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.HubWorkItems
{
    public class HubWorkItemResultsDto
    {
        public int Count { get; set; }
        public List<HubWorkItemDto> WorkItems { get; set; } = new List<HubWorkItemDto>();
    }
}
