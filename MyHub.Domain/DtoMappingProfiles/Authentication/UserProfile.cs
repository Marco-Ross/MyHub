using AutoMapper;
using MyHub.Domain.Users;
using MyHub.Domain.Users.UsersDto;

namespace MyHub.Domain.DtoMappingProfiles.Authentication
{
    public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<LoginUserDto, User>();
			CreateMap<RegisterUserDto, User>();
			CreateMap<User, HubUserDto>();
		}
	}
}
