using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Feedback.FeedbackDto;
using MyHub.Domain.Feedback.Interfaces;

namespace MyHub.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FeedbackController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IFeedbackService _feedbackService;

		public FeedbackController(IFeedbackService feedbackService, IMapper mapper)
		{
			_feedbackService = feedbackService;
			_mapper = mapper;
		}

		[Authorize]
		[HttpPost]
		public IActionResult PostFeedback(UserFeedbackDto feedbackDto)
		{
			var feedback = _feedbackService.PostFeedback(UserId, feedbackDto.Description);

			if (feedback is null)
				return BadRequest("Failed to post feedback.");

			return Ok(_mapper.Map<UserFeedbackDto>(feedback));
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpGet]
		public IActionResult GetFeedback()
		{
			var feedback = _feedbackService.GetFeedback();

			if (feedback is null)
				return BadRequest("Failed to get feedback.");

			return Ok(new { UserFeedback = _mapper.Map<IEnumerable<AllUserFeedbackDto>>(feedback) });
		}
		
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public IActionResult DeleteFeedback(string id)
		{
			var deleted = _feedbackService.DeleteFeedback(id);

			if (!deleted)
				return BadRequest("Failed to get feedback.");

			return Ok();
		}
	}
}
