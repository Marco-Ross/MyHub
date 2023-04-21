namespace MyHub.Domain.Integration.AzureDevOps.HubWorkItems
{
	public class HubWorkItemDto
	{
		public int Id { get; set; }
		public int Rev { get; set; }
		public HubWorkItemFieldsDto? Fields { get; set; }
		public string Url { get; set; } = string.Empty;
	}
}
