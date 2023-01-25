namespace MyHub.Domain.Authentication
{
	public class User
	{
		public Guid Id { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
	}

	public class UserDto
	{
		public Guid Id { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
	}
}
