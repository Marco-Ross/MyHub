using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users.Google;

namespace MyHub.Domain.Authentication.Google
{
	public interface IGoogleAuthenticationService : ISharedAuthService
	{
		Task<GoogleUser> ExchangeAuthCode(string authUser, string authCode);
	}
}
