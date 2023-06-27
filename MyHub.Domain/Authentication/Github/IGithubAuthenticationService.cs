using MyHub.Domain.Users.Github;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Github
{
	public interface IGithubAuthenticationService
	{
		Task<Validator<GithubUser>> ExchangeAuthCode(string authCode);
	}
}
