using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ChangeEmailDto
	{
		[Required]
		public string Email { get; set; } = string.Empty;
		[Required]
		public string IdToken { get; set; } = string.Empty;
	}
}
