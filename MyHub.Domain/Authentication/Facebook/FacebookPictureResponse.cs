using System.Text.Json.Serialization;

namespace MyHub.Domain.Authentication.Facebook
{
	public class FacebookPictureResponse
	{
		[JsonPropertyName("height")]
		public int Height { get; set; }

		[JsonPropertyName("is_silhouette")]
		public bool IsSilhouette { get; set; }

		[JsonPropertyName("url")]
		public string Url { get; set; } = string.Empty;

		[JsonPropertyName("width")]
		public int Width { get; set; }
	}
}
