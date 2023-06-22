namespace MyHub.Domain.Authentication.AuthenticationDto
{
	public class GoogleAccessOptionsDto
	{
		public string AuthUser { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
		public string Prompt { get; set; } = string.Empty;
		public string Scope { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public string Nonce { get; set; } = string.Empty;
	}
}
