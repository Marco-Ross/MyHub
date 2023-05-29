using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ChangeEmailCompleteDto
	{
		[Required]
		public string UserId { get; set; } = string.Empty;

		[Required]
		public string ChangeEmailToken { get; set; } = string.Empty;
	}
}
