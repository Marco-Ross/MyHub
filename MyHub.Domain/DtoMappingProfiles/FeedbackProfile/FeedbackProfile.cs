using AutoMapper;
using MyHub.Domain.Feedback;
using MyHub.Domain.Feedback.FeedbackDto;

namespace MyHub.Domain.DtoMappingProfiles.FeedbackProfile
{
	public class FeedbackProfile : Profile
	{
		public FeedbackProfile()
		{
			CreateMap<UserFeedbackDto, UserFeedback>();
			CreateMap<UserFeedback, UserFeedbackDto>();

			CreateMap<UserFeedback, AllUserFeedbackDto>()
				.ForPath(x => x.Username, m => m.MapFrom(u => u.UserCreated != null ? u.UserCreated.Username : string.Empty))
				.ForPath(x => x.UserId, m => m.MapFrom(u => u.UserCreated != null ? u.UserCreated.Id : string.Empty));
		}
	}
}
