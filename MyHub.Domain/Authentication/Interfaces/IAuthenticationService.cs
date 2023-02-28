using MyHub.Domain.Users;

namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IAuthenticationService
	{
		bool RegisterUser(User user);
		LoginDetails? AuthenticateUser(string username, string password);
		LoginDetails? RefreshUserAuthentication(string accessToken, string refreshToken);
		bool RevokeUser(string userId);
	}
}
