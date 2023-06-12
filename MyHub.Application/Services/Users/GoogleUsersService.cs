using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace MyHub.Application.Services.Users
{
	public class GoogleUsersService : IGoogleUsersService
	{
		public IUsersService _hubUsersService;
		private readonly HttpClient _httpClient;

		public GoogleUsersService(IUsersService hubUsersService, HttpClient httpClient)
		{
			_hubUsersService = hubUsersService;
			_httpClient = httpClient;
		}
		public async Task<Stream?> GetUserProfileImage(string userId)
		{
			var user = _hubUsersService.GetFullAccessingUserById(userId);

			if (user is null || string.IsNullOrWhiteSpace(user.ThirdPartyIdToken))
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(user.ThirdPartyIdToken);

			var pictureClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomJwtClaimNames.Picture.Name)?.Value;

			if (pictureClaim is null) return null;

			return await _httpClient.GetStreamAsync(pictureClaim);
		}

		public AccessingUser? GetFullAccessingUserById(string userId)
		{
			var user = _hubUsersService.GetFullAccessingUserById(userId);

			if (user is null || string.IsNullOrWhiteSpace(user.ThirdPartyIdToken))
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(user.ThirdPartyIdToken);

			user.User.Username = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value ?? string.Empty;
			user.User.Name = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value ?? string.Empty;
			user.User.Surname = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value ?? string.Empty;
			user.Email = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;

			return user;
		}
	}
}
