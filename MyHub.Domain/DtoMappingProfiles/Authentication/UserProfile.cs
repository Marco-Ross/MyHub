using AutoMapper;
using MyHub.Domain.Users;

namespace MyHub.Domain.DtoMappingProfiles.Authentication
{
    public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<LoginUserDto, User>();
			CreateMap<User, LoginUserDto>();
		}
	}
}
