using MyHub.Domain.Users.Google;
using MyHub.Domain.Users.Interfaces;
using System.Text.RegularExpressions;

namespace MyHub.Application.Services.Users
{
	public partial class GoogleUsersService : IGoogleUsersService
	{
		public IUsersService _hubUsersService;
		private readonly HttpClient _httpClient;

		public GoogleUsersService(IUsersService hubUsersService, HttpClient httpClient)
		{
			_hubUsersService = hubUsersService;
			_httpClient = httpClient;
		}

		public async Task<Stream?> GetUserProfileImage(string pictureUrl)
		{
			var responseStream = await _httpClient.GetStreamAsync(ChangeImageSize(pictureUrl));
			var stream = new MemoryStream();
			await responseStream.CopyToAsync(stream);

			return stream;
		}

		private static string ChangeImageSize(string pictureUrl)
			=> ChangeImageSizeRegex().Replace(pictureUrl, "300");

		[GeneratedRegex("(?<=s)\\d+(?=-c)", RegexOptions.Compiled)]
		private static partial Regex ChangeImageSizeRegex();
	}
}
