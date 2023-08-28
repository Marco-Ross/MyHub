using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Feedback;
using MyHub.Domain.Feedback.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Feedback
{
	public class FeedbackService : IFeedbackService
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public FeedbackService(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		public IEnumerable<UserFeedback> GetFeedback() => _applicationDbContext.Feedback.Include(x => x.UserCreated);

		public UserFeedback? PostFeedback(string userId, string description)
		{
			if (string.IsNullOrWhiteSpace(description))
				return null;

			var user = _applicationDbContext.Users.Single(x => x.Id == userId);

			var savedEntity = _applicationDbContext.Feedback.Add(new UserFeedback
			{
				Id = Guid.NewGuid().ToString(),
				Description = description,
				UserCreated = user,
				CreatedDate = DateTime.Now
			});

			_applicationDbContext.SaveChanges();

			return savedEntity.Entity;
		}

		public bool DeleteFeedback(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return false;

			var feedback = _applicationDbContext.Feedback.Single(x => x.Id == id);

			_applicationDbContext.Feedback.Remove(feedback);

			_applicationDbContext.SaveChanges();

			return true;
		}
	}
}
