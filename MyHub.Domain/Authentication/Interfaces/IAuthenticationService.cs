using MyHub.Domain.Users;

namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IAuthenticationService
	{
		User? RegisterUser(User user);
		Tokens? AuthenticateUser(string username, string password);
		Tokens? RefreshUserAuthentication(string accessToken, string refreshToken);		
	}
}
