namespace MyHub.Domain.Feedback.Interfaces
{
	public interface IFeedbackService
	{
		UserFeedback? PostFeedback(string userId, string description);

		IEnumerable<UserFeedback> GetFeedback();
		bool DeleteFeedback(string id);
	}
}
