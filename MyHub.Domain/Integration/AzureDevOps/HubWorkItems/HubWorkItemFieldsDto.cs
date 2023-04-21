namespace MyHub.Domain.Integration.AzureDevOps.HubWorkItems
{
	public class HubWorkItemFieldsDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string WorkItemType { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public string CreatedDate { get; set; } = string.Empty;
		public string ChangedDate { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
