using Facebook;
using MyHub.Domain.Authentication.Facebook;
using MyHub.Domain.Users.Facebook;
using MyHub.Domain.Users.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Authentication;

namespace MyHub.Application.Services.Users
{
	public class FacebookUsersService : IFacebookUsersService
	{
		public IUsersService _hubUsersService;
		private readonly HttpClient _httpClient;
		private readonly AuthenticationOptions _authOptions;
		private readonly FacebookClient _facebookClient;

		public FacebookUsersService(IUsersService hubUsersService, HttpClient httpClient, IOptions<AuthenticationOptions> authOptions)
		{
			_hubUsersService = hubUsersService;
			_httpClient = httpClient;
			_authOptions = authOptions.Value;
			_facebookClient = new FacebookClient();
		}
		public async Task<Stream?> GetUserProfileImage(string pictureUrl)
		{
			var responseStream = await _httpClient.GetStreamAsync(pictureUrl);
			var stream = new MemoryStream();
			await responseStream.CopyToAsync(stream);

			return stream;
		}

		public async Task<FacebookDetailsResponse?> GetUserDetails(string accessToken)
		{
			var parameters = new Dictionary<string, object>
			{
				["client_id"] = _authOptions.ThirdPartyLogin.Facebook.ClientId,
				["client_secret"] = _authOptions.ThirdPartyLogin.Facebook.ClientSecret,
				["access_token"] = accessToken,
				["fields"] = "id,name,email,picture.width(300).height(300),first_name,last_name",
			};

			try
			{
				var resultTask = await _facebookClient.GetTaskAsync("/me", parameters);

				if (resultTask is null)
					return null;

				var facebookAccessResponse = JsonSerializer.Deserialize<FacebookDetailsResponse>(resultTask.ToString() ?? string.Empty);

				if (facebookAccessResponse is null || !string.IsNullOrWhiteSpace(facebookAccessResponse?.Error))
					return null;

				return facebookAccessResponse;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
