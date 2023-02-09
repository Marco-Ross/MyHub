using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Api.Controllers
{
    [Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
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

		[AllowAnonymous]
		[HttpPost]
		public User Post(UserDto userDto)
		{
			return _userService.CreateUser(_mapper.Map<User>(userDto));
		}

		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
