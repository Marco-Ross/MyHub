using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users.Facebook;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Facebook
{
	public interface IFacebookAuthenticationService
	{
		Task<Validator<FacebookUser>> ExchangeAuthCode(string authCode);
	}
}
