using AutoMapper;
using MyHub.Domain.Integration.AzureDevOps.HubWorkItems;
using MyHub.Domain.Integration.AzureDevOps.WorkItems;

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
