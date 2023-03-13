namespace MyHub.Domain.Users.UsersDto
{
	public class ResetPasswordCompleteDto
	{
		public string UserId { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ResetPasswordToken { get; set; } = string.Empty;
	}
}
