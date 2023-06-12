using Microsoft.AspNetCore.Mvc;
using MyHub.Api.Filters;
using MyHub.Domain.Hubs.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.WebHooks;

namespace MyHub.Api.Controllers
{
    //Use NgRok Ip as webhook server to localhost.
    [ApiController]
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("[controller]")]
	public class AzureDevOpsWebHooksController : BaseController
	{
		private readonly IAzureDevOpsCacheService _azureDevOpsCacheService;
		private readonly IHubResolver<UpdatedWorkItemEventDto> _hubResolver;

		public AzureDevOpsWebHooksController(IAzureDevOpsCacheService azureDevOpsCacheService, IHubResolver<UpdatedWorkItemEventDto> hubResolver)
		{
			_azureDevOpsCacheService = azureDevOpsCacheService;
			_hubResolver = hubResolver;
		}

		[HttpPost]
		public async Task<IActionResult> WorkItemUpdate(UpdatedWorkItemEventDto updatedWorkItemEvent)
		{
			_azureDevOpsCacheService.UpdateCachedWorkItems(updatedWorkItemEvent.UpdatedWorkItemDto.WorkItemId, updatedWorkItemEvent.UpdatedWorkItemDto.UpdatedWorkItemFields);

			await _hubResolver.Send(updatedWorkItemEvent);
			return Ok();
		}
	}
}