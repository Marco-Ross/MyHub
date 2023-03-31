namespace MyHub.Domain.ConfigurationOptions.Authentication
{
	public class Cookies
	{
		public string Domain { get; set; } = string.Empty;
		public string CsrfToken { get; set; } = string.Empty;
	}
}
