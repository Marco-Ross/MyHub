using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;

namespace MyHub.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AzureDevOpsController : BaseController
	{
		private readonly IAzureDevOpsService _azureDevOpsService;

		public AzureDevOpsController(IAzureDevOpsService azureDevOpsService)
		{
			_azureDevOpsService = azureDevOpsService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _azureDevOpsService.GetWorkItems());
		}
	}
}
