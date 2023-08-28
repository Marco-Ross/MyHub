using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Feedback
{
	public class UserFeedback
	{
		public string Id { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }

		[ForeignKey("UserCreated")]
		public string? UserCreatedId { get; set; }
		public User? UserCreated { get; set; }
	}
}
