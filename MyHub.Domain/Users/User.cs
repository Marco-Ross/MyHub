using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users
{
	public class User
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Theme { get; set; } = string.Empty;
	}
}
