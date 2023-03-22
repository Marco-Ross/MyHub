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
			CreateMap<RegisterUserDto, AccessingUser>().ForPath(x => x.User.Username, m => m.MapFrom(u => u.Username));
			CreateMap<AccessingUser, HubUserDto>();
		}
	}
}
