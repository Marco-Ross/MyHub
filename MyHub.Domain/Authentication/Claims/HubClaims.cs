namespace MyHub.Domain.Authentication.Claims
{
    public class HubClaims
    {
        public string Sub { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Iat { get; set; } = string.Empty;
        public string Jti { get; set; } = string.Empty;
        public string Iss { get; set; } = string.Empty;
        public string IssManaging { get; set; } = string.Empty;
        public string Aud { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
    }
}
