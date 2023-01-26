using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using System.Security.Cryptography;

namespace MyHub.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IAuthenticationService _authenticationService;

		public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper)
		{
			_authenticationService = authenticationService;
			_mapper = mapper;
		}

		[HttpPost]
		public User Post([FromBody] UserDto userDto)
		{
			return _authenticationService.CreateUser(_mapper.Map<User>(userDto));
		}

		[HttpPost]
		public IActionResult PostLogin([FromBody] UserDto userDto)
		{
			return Ok(_authenticationService.AuthenticateUser(_mapper.Map<User>(userDto)));
		}

		[HttpGet("GetKey")]
		public string GetIsWorking()
		{
			var key = new byte[32];
			RNGCryptoServiceProvider.Create().GetBytes(key);
			var base64Secret = Convert.ToBase64String(key);
			// make safe for url
			var urlEncoded = base64Secret.TrimEnd('=').Replace('+', '-').Replace('/', '_');

			return urlEncoded;
		}
	}
}