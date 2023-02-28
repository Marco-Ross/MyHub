using MyHub.Domain.Users.UsersDto;

namespace MyHub.Domain.Authentication
{
	public class LoginDetails
	{
		public Tokens? Tokens { get; set; }
		public HubUserDto? HubUserDto { get; set; }
	}
}
