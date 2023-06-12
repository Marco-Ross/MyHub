namespace MyHub.Domain.Users.UsersDto
{
	public class AccountSettingsUserDto
	{
		public string Id { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public DateTime? EmailVerificationDate { get; set; } = null;	
	}
}
