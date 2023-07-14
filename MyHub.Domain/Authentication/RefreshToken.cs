using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Authentication
{
	public class RefreshToken
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; } = string.Empty;
		public AccessingUser User { get; set; } = new AccessingUser();
	}
}
