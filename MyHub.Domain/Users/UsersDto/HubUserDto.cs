namespace MyHub.Domain.Users.UsersDto
{
	public class HubUserDto
	{
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public bool IsLoggedIn => true;
	}
}
