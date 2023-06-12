using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ResetPasswordLoggedInDto
	{
		[Required]
		public string OldPassword { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}
}
