using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Users.UsersDto;
using System.Text.Json;

namespace MyHub.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IUsersService _userService;
		private readonly AuthenticationOptions _authOptions;

		public UsersController(IUsersService userService, IMapper mapper, IOptions<AuthenticationOptions> authOptions)
		{
			_authOptions = authOptions.Value;
			_userService = userService;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(_mapper.Map<AccountSettingsUserDto>(_userService.GetFullAccessingUserById(UserId)));
		}

		[HttpPut]
		public IActionResult Put(AccountSettingsUserUpdateDto accountSettingsUserDto)
		{
			_userService.UpdateUserAccount(_mapper.Map<AccessingUser>(accountSettingsUserDto), UserId);

			var existingCookie = Request.Cookies[AuthConstants.LoggedIn];

			if (existingCookie is null)
				return Ok();

			var loginCookie = JsonSerializer.Deserialize<HubUserDto>(existingCookie);

			if (loginCookie is null)
				return Ok();

			loginCookie.Username = accountSettingsUserDto.Username;

			Response.Cookies.Append(AuthConstants.LoggedIn, JsonSerializer.Serialize(loginCookie));

			return Ok();
		}

		[HttpDelete()]
		public IActionResult Delete()
		{
			return Ok();
		}

		[HttpPut("Theme")]
		public IActionResult UpdateUserTheme(ThemeOptionsDto themeOptionsDto)
		{
			_userService.UpdateUserTheme(UserId, themeOptionsDto.Theme);

			return Ok();
		}

		[HttpGet("Theme")]
		public IActionResult GetUserTheme()
		{
			return Ok(new { Theme = _userService.GetUserTheme(UserId) });
		}

		[HttpGet("ProfileImage")]
		public async Task<IActionResult> GetProfileImage()
		{
			var image = await _userService.GetUserProfileImage(UserId);

			if (image is null)
				return Ok();

			return File(image, "image/png");
		}
		
		[HttpPut("ProfileImage")]
		public async Task<IActionResult> UpdateProfileImage(ProfileImageDto imageDto)
		{
			var imageUploaded = await _userService.UpdateUserProfileImage(UserId, imageDto.Image);

			if (!imageUploaded)
				return BadRequest("Unable to upload image.");

			return Ok();
		}

		[HttpDelete("ProfileImage")]
		public async Task<IActionResult> DeleteProfileImage()
		{
			var imageDeleted = await _userService.DeleteUserProfileImage(UserId);

			if (!imageDeleted)
				return BadRequest("Unable to remove image.");

			return Ok();
		}
	}
}
