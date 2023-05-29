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
		public string Name { get; set; } = string.Empty;
		[Required]
		public string Surname { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
		public string ProfileImage { get; set; } = string.Empty;
    }
}
