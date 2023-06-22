using System.Text.Json.Serialization;

namespace MyHub.Domain.Authentication.Facebook
{
	public class FacebookAccessResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; } = string.Empty;

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; } = string.Empty;

		[JsonPropertyName("expires_in")]
		public int ExpiresInSeconds { get; set; }

		public DateTime ExpiresInDate { get => DateTime.Now.AddSeconds(ExpiresInSeconds); }

		[JsonPropertyName("auth_type")]
		public string AuthType { get; set; } = string.Empty;

		[JsonPropertyName("error")]
		public string Error { get; set; } = string.Empty;
		
		[JsonPropertyName("success")]
		public bool IsSuccessful { get; set; }
	}
}
