using MyHub.Domain.Users.Github;
using MyHub.Domain.Users.Interfaces;
using Octokit;

namespace MyHub.Application.Services.Users
{
	public class GithubUsersService : IGithubUsersService
	{
		public IUsersService _hubUsersService;
		private readonly HttpClient _httpClient;
		private readonly IGitHubClient _githubClient;

		public GithubUsersService(IUsersService hubUsersService, HttpClient httpClient, IGitHubClient githubClient)
		{
			_hubUsersService = hubUsersService;
			_httpClient = httpClient;
			_githubClient = githubClient;
		}
		public async Task<Stream?> GetUserProfileImage(string pictureUrl)
		{
			var responseStream = await _httpClient.GetStreamAsync(pictureUrl);
			var stream = new MemoryStream();
			await responseStream.CopyToAsync(stream);

			return stream;
		}
	}
}
