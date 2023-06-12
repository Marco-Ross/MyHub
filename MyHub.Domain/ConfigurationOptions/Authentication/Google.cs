namespace MyHub.Domain.ConfigurationOptions.Authentication
{
	public class Google
	{
		public string Issuer { get; set; } = string.Empty;
		public string Audience { get; set; } = string.Empty;
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string RedirectUri { get; set; } = string.Empty;
	}
}
