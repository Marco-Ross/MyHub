using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class AccountSettingsUserUpdateDto
	{
		[Required, StringLength(30)]
		public string Username { get; set; } = string.Empty;
		[Required, StringLength(30)]
		public string Name { get; set; } = string.Empty;
		[Required, StringLength(30)]
		public string Surname { get; set; } = string.Empty;
	}
}
