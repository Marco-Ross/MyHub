namespace MyHub.Domain.ConfigurationOptions.Authentication
{
    public class AuthenticationOptions
    {
        public Jwt JWT { get; set; } = new Jwt();
        public Cookies Cookies { get; set; } = new Cookies();
        public AuthEmailSenderOptions AuthEmailSenderOptions { get; set; } = new AuthEmailSenderOptions();
        public Pat PAT { get; set; } = new Pat();
        public string ApiKey { get; set; } = string.Empty;
    }
}
