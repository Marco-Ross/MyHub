using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
	public class ProfileImageDto
	{
		[Required]
		public string Image { get; set; } = string.Empty;
	}
}
