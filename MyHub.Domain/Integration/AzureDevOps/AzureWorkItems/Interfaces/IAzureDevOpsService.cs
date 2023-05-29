using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.WorkItems;
using System.Text.Json.Nodes;

namespace MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.Interfaces
{
    public interface IAzureDevOpsService
    {
        Task<WorkItemResults> GetWorkItems();
    }
}
