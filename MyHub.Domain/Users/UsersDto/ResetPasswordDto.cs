using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ResetPasswordDto
	{
		[Required]
		public string Email { get; set; } = string.Empty;
	}
}
