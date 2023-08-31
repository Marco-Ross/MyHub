namespace MyHub.Domain.Feedback.FeedbackDto
{
	public class AllUserFeedbackDto
	{
		public string Id { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
	}
}
