using Microsoft.AspNetCore.Mvc;
using MyHub.Api.Filters;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.WebHookDto;

namespace MyHub.Api.Controllers
{
	[ApiController]
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("[controller]")]
	public class AzureDevOpsWebHooksController : BaseController
	{
		private readonly IAzureDevOpsService _azureDevOpsService;

		public AzureDevOpsWebHooksController(IAzureDevOpsService azureDevOpsService)
		{
			_azureDevOpsService = azureDevOpsService;
		}

		[HttpPost]
		public IActionResult WorkItemUpdate(UpdatedWorkItemEventDto updatedWorkItemEvent)
		{
			return Ok();
		}
	}
}
