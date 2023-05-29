using Autofac;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;

namespace MyHub.Application.Services.Emails.EmailConstructors
{
    public class EmailConstructorFactory : IEmailConstructorFactory
	{
		private readonly IComponentContext _container;

		public EmailConstructorFactory(IComponentContext container)
		{
			_container = container;
		}

		public IEmailConstructorService ConstructNewEmailService<T>()
		{
			if (typeof(T) == typeof(AccountRegisterEmail))
				return _container.Resolve<AccountRegisterEmailConstructor>();

			if (typeof(T) == typeof(PasswordRecoveryEmail))
				return _container.Resolve<PasswordEmailConstructor>();
			
			if (typeof(T) == typeof(EmailChangeEmail))
				return _container.Resolve<ChangeEmailConstructor>();

			else
				return null;//default email
		}
	}
}
