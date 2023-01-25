using AutoMapper;
using MyHub.Domain.Authentication;

namespace MyHub.Domain.DtoMappingProfiles.Authentication
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, UserDto>();
			CreateMap<UserDto, User>();
		}
	}
}
