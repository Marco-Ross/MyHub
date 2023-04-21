using MyHub.Domain.Integration.AzureDevOps.WorkItems;

namespace MyHub.Domain.Integration.AzureDevOps.Interfaces
{
    public interface IAzureDevOpsService
	{
		Task<WorkItemResults> GetWorkItems();
	}
}
