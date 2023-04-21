using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Integration.AzureDevOps.HubWorkItems;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;

namespace MyHub.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AzureDevOpsController : BaseController
	{
		private readonly IAzureDevOpsService _azureDevOpsService;
		private readonly IMapper _mapper;

		public AzureDevOpsController(IAzureDevOpsService azureDevOpsService, IMapper mapper)
		{
			_azureDevOpsService = azureDevOpsService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var workItems = await _azureDevOpsService.GetWorkItems();
			return Ok(_mapper.Map<HubWorkItemResultsDto>(workItems));
		}
	}
}
