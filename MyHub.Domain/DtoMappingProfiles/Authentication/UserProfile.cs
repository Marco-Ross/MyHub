using AutoMapper;
using MyHub.Domain.Users;
using MyHub.Domain.Users.UsersDto;

namespace MyHub.Domain.DtoMappingProfiles.Authentication
{
    public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<LoginUserDto, AccessingUser>();
			CreateMap<RegisterUserDto, AccessingUser>();
			CreateMap<AccessingUser, HubUserDto>();
		}
	}
}
