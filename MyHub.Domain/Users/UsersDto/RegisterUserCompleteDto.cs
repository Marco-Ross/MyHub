using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class RegisterUserCompleteDto
	{
		[Required]
		public string UserId { get; set; } = string.Empty;
		[Required]
		public string RegisterToken { get; set; } = string.Empty;
	}
}
