using MyHub.Domain.Integration.AzureDevOps.WorkItems;
using System.Text.Json.Nodes;

namespace MyHub.Domain.Integration.AzureDevOps.Interfaces
{
    public interface IAzureDevOpsService
	{
		Task<WorkItemResults> GetWorkItems();
	}
}
