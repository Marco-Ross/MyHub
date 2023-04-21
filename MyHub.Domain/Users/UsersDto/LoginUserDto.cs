using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
    public class LoginUserDto
    {
		[Required]
		public string Email { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
    }
}
