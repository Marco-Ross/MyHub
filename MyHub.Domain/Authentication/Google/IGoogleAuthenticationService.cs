using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Google
{
	public interface IGoogleAuthenticationService
	{
		Task<Validator<GoogleUser>> ExchangeAuthCode(string authUser, string authCode, string nonce);
	}
}
