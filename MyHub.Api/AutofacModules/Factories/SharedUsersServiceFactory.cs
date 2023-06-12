using Autofac;
using Microsoft.Extensions.Options;
using MyHub.Domain.Authentication.Google;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users.Interfaces;

namespace MyHub.Api.AutofacModules.Factories
{
    public class SharedUsersServiceFactory: ISharedAuthServiceFactory
	{
		private readonly IComponentContext _container;
		private readonly AuthenticationOptions _authenticationOptions;

		public SharedUsersServiceFactory(IComponentContext container, IOptions<AuthenticationOptions> authenticationOptions)
		{
			_container = container;
			_authenticationOptions = authenticationOptions.Value;
		}

		public ISharedUsersService GetUsersService(string issuer)
		{
			if (issuer == _authenticationOptions.ThirdPartyLogin.Google.Issuer)
				return _container.Resolve<IGoogleUsersService>();

			return _container.Resolve<IUsersService>();
		}
		
		public ISharedAuthService GetAuthService(string issuer)
		{
			if (issuer == _authenticationOptions.ThirdPartyLogin.Google.Issuer)
				return _container.Resolve<IGoogleAuthenticationService>();

			return _container.Resolve<IAuthenticationService>();
		}
	}
}
