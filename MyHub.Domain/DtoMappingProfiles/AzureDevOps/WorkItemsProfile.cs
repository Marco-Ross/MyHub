using AutoMapper;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.HubWorkItems;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.WorkItems;

namespace MyHub.Domain.DtoMappingProfiles.AzureDevOps
{
    public class WorkItemsProfile : Profile
	{
		public WorkItemsProfile()
		{
			CreateMap<WorkItemResults, HubWorkItemResultsDto>();
			CreateMap<WorkItem, HubWorkItemDto>();
			CreateMap<WorkItemFields, HubWorkItemFieldsDto>();
		}
	}
}
