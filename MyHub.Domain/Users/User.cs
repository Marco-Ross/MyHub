namespace MyHub.Domain.Users
{
	public class User
    {
        public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string Salt { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		//public bool IsEmailVerified { get; set; }
	}
}
