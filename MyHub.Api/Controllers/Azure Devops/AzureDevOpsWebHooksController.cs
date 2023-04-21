using Microsoft.AspNetCore.Mvc;
using MyHub.Api.Filters;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.WebHooks;

namespace MyHub.Api.Controllers
{
	//Use NgRok Ip as webhook server to localhost.
	[ApiController]
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("[controller]")]
	public class AzureDevOpsWebHooksController : BaseController
	{
		private readonly IAzureDevOpsCacheService _azureDevOpsCacheService;

		public AzureDevOpsWebHooksController(IAzureDevOpsCacheService azureDevOpsCacheService)
		{
			_azureDevOpsCacheService = azureDevOpsCacheService;
		}

		[HttpPost]
		public async Task<IActionResult> WorkItemUpdate(UpdatedWorkItemEventDto updatedWorkItemEvent)
		{
			await _azureDevOpsCacheService.UpdateCache(updatedWorkItemEvent.UpdatedWorkItemDto.WorkItemId, updatedWorkItemEvent.UpdatedWorkItemDto.UpdatedWorkItemFields);
			return Ok();
		}
	}
}
