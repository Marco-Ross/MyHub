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

			CreateMap<AccessingUser, HubUserDto>().ForPath(x => x.Username, m => m.MapFrom(u => u.User.Username));

			CreateMap<AccessingUser, AccountSettingsUserDto>()
				.ForPath(x => x.Username, m => m.MapFrom(u => u.User.Username))
				.ForPath(x => x.Name, m => m.MapFrom(u => u.User.Name))
				.ForPath(x => x.Surname, m => m.MapFrom(u => u.User.Surname));

			CreateMap<AccountSettingsUserUpdateDto, AccessingUser>()
				.ForPath(x => x.User.Username, m => m.MapFrom(u => u.Username))
				.ForPath(x => x.User.Name, m => m.MapFrom(u => u.Name))
				.ForPath(x => x.User.Surname, m => m.MapFrom(u => u.Surname));
		}
	}
}
