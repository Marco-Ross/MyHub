using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ResetPasswordCompleteDto
	{
		[Required]
		public string UserId { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
		[Required]
		public string ResetPasswordToken { get; set; } = string.Empty;
	}
}
