using AutoMapper;
using MyHub.Domain.Users;

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
