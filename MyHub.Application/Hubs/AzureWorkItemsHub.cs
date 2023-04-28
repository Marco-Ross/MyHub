using Microsoft.AspNetCore.SignalR;
using MyHub.Domain.Hubs.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.WebHooks;

namespace MyHub.Application.Hubs
{
	public class AzureWorkItemsHub : Hub, IHubResolver<UpdatedWorkItemEventDto>
	{
		private readonly IHubContext<AzureWorkItemsHub> _hub;
		public AzureWorkItemsHub(IHubContext<AzureWorkItemsHub> hubContext)
		{
			_hub = hubContext;
		}
		public async Task Send(UpdatedWorkItemEventDto updatedWorkItem)
		{
			await _hub.Clients.All.SendAsync("UpdatedWorkItem", updatedWorkItem);
		}
	}
}