using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Domain.Authentication.Google
{
    public interface ISharedAuthServiceFactory
    {
        ISharedUsersService GetUsersService(string issuer);
        ISharedAuthService GetAuthService(string issuer);
    }
}
