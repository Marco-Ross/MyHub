using MyHub.Domain.Users.UsersDto;

namespace MyHub.Domain.Authentication
{
	public class LoginDetails
	{
		public Tokens Tokens { get; set; } = new Tokens();
		public HubUserDto HubUserDto { get; set; } = new HubUserDto();
	}
}
