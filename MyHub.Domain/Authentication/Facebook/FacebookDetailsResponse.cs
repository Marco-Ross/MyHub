using System.Text.Json.Serialization;

namespace MyHub.Domain.Authentication.Facebook
{
	public class FacebookDetailsResponse
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;
		
		[JsonPropertyName("email")]
		public string Email { get; set; } = string.Empty;

		[JsonPropertyName("picture")]
		public FacebookPictureDataResponse PictureData { get; set; } = new FacebookPictureDataResponse();

		[JsonPropertyName("first_name")]
		public string FirstName { get; set; } = string.Empty;

		[JsonPropertyName("last_name")]
		public string LastName { get; set; } = string.Empty;

		[JsonPropertyName("error")]
		public string Error { get; set; } = string.Empty;
	}
}
