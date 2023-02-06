namespace MyHub.Domain.Authentication
{
	public class User
	{
		public string Id { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		//public string Email { get; set; } = string.Empty;
	}

	public class UserDto
	{
		public string Id { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		//public string Email { get; set; } = string.Empty;
	}
}
