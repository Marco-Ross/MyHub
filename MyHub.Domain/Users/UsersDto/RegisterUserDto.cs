using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users.UsersDto
{
    public class RegisterUserDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
		[Required]
		public string Username { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
    }
}
