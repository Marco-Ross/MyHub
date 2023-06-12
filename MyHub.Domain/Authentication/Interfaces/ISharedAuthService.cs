using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Interfaces
{
    public interface ISharedAuthService
    {
        Validator<LoginDetails> RefreshUserAuthentication(string idToken, string refreshToken);
    }
}
