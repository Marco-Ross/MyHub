namespace MyHub.Domain.Integration.AzureDevOps.Interfaces
{
	public interface IAzureDevOpsService
	{
		Task<WorkItemResults> GetWorkItems();
	}
}
