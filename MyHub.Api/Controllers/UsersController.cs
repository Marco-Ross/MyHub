using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Users.UsersDto;

namespace MyHub.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;

		public UsersController(IUserService userService, IMapper mapper)
		{
			_userService = userService;
			_mapper = mapper;
		}

		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
		
		[HttpGet("ProfileImage")]
		public async Task<IActionResult> GetProfileImage()
		{
			var image = await _userService.GetUserProfileImage(UserId);

			if(image is null)
				return Ok();

			return File(image, "image/png");
		}
	}
}
