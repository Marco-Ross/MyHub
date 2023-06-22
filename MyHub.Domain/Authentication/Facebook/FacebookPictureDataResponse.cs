using System.Text.Json.Serialization;

namespace MyHub.Domain.Authentication.Facebook
{
	public class FacebookPictureDataResponse
	{
		[JsonPropertyName("data")]
		public FacebookPictureResponse Picture { get; set; } = new FacebookPictureResponse();
	}
}
