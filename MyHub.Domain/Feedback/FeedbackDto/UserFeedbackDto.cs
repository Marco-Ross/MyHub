using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Feedback.FeedbackDto
{
	public class UserFeedbackDto
	{
		public string Id { get; set; } = string.Empty;

		[Required, StringLength(1000)]
		public string Description { get; set; } = string.Empty;
	}
}
