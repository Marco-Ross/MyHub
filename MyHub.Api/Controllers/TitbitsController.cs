using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Api.Authorization;
using MyHub.Domain.DtoMappingProfiles.Gallery;
using MyHub.Domain.Titbits;
using MyHub.Domain.Titbits.Interfaces;
using MyHub.Domain.Titbits.TitbitsDto;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TitbitsController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly ITitbitsService _titbitsService;
		private readonly IMarcoService _marcoService;

		public TitbitsController(IMapper mapper, ITitbitsService titbitsService, IMarcoService marcoService)
		{
			_mapper = mapper;
			_titbitsService = titbitsService;
			_marcoService = marcoService;
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost]
		public IActionResult AddNewTitbit(AddTitbitDto addTitbitDto)
		{
			var titbit = _titbitsService.AddNewTitbit(_marcoService.GetMarcoId(), _mapper.Map<Titbit>(addTitbitDto));

			if (titbit is null)
				return BadRequest("Failed to add titbit.");

			return Ok(_mapper.Map<TitbitDto>(titbit));
		}
		
		[Authorize(Policy = "AdminOnly")]
		[HttpPut]
		public IActionResult UpdateTitbit(UpdateTitbitDto updateTitbitDto)
		{
			var titbit = _titbitsService.UpdateTitbit(_marcoService.GetMarcoId(), _mapper.Map<Titbit>(updateTitbitDto));

			if (titbit is null)
				return BadRequest("Failed to update titbit.");

			return Ok(_mapper.Map<TitbitDto>(titbit));
		}
		
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{titbitId}")]
		public IActionResult DeleteTitbit(string titbitId)
		{
			var deleted = _titbitsService.DeleteTitbit(titbitId);

			if (!deleted)
				return BadRequest("Failed to delete titbit.");

			return Ok();
		}

		[AuthorizeLoggedIn]
		[HttpGet]
		public IActionResult GetTitbits()
		{
			var titbit = _titbitsService.GetTitbits(_marcoService.GetMarcoId(), UserId).AsEnumerable();

			if (titbit is null)
				return BadRequest("Failed to add titbit.");

			return Ok(new { Titbits = _mapper.Map<IEnumerable<TitbitDto>>(titbit, opt => opt.Items[GalleryContextOptions.UserId] = UserId) });
		}

		[Authorize]
		[HttpPost("Like")]
		public IActionResult LikeTitbit(LikeTitbitDto likeTitbitDto)
		{
			var imageLiked = _titbitsService.LikeTitbit(UserId, likeTitbitDto.TitbitId);

			if (!imageLiked)
				return BadRequest("Titbit already liked.");

			return Ok();
		}

		[Authorize]
		[HttpPost("Unlike")]
		public IActionResult UnlikeTitbit(UnlikeTitbitDto unlikeTitbitDto)
		{
			var imageLiked = _titbitsService.UnlikeTitbit(UserId, unlikeTitbitDto.TitbitId);

			if (!imageLiked)
				return BadRequest("Titbit already unliked.");

			return Ok();
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("Categories")]
		public IActionResult AddNewTitbitCategory(AddTitbitCategoryDto addTitbitCategoryDto)
		{
			var titbits = _titbitsService.AddNewTitbitCategories(_mapper.Map<List<TitbitCategory>>(addTitbitCategoryDto.Categories));

			if (!titbits.Any())
				return BadRequest("Failed to add categories.");

			return Ok(new { Categories = _mapper.Map<List<TitbitCategoryDto>>(titbits) });
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPut("Categories")]
		public IActionResult UpdateTitbitCategory(UpdateTitbitCategoryDto updateTitbitCategoryDto)
		{
			var titbits = _titbitsService.UpdateTitbitCategories(_mapper.Map<List<TitbitCategory>>(updateTitbitCategoryDto.Categories));

			if (!titbits.Any())
				return BadRequest("Failed to add categories.");

			return Ok(new { Categories = _mapper.Map<List<TitbitCategoryDto>>(titbits) });
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("Categories/Delete")]
		public IActionResult DeleteTitbitCategory(DeleteTitbitCategoryDto deleteTitbitCategoryDto)
		{
			_titbitsService.DeleteTitbitCategories(_mapper.Map<List<TitbitCategory>>(deleteTitbitCategoryDto.Categories));

			return Ok();
		}

		[HttpGet("Categories")]
		public IActionResult GetTitbitCategories()
		{
			var categories = _titbitsService.GetTitbitCategories().AsEnumerable();

			if (categories is null)
				return BadRequest("Failed to get titbit categories.");

			return Ok(new { Categories = _mapper.Map<IEnumerable<TitbitCategoryDto>>(categories) });
		}
	}
}
