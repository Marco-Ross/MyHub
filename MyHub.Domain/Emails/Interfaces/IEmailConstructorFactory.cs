using MyHub.Application.Services.Emails;

namespace MyHub.Domain.Emails.Interfaces
{
	public interface IEmailConstructorFactory
	{
		public IEmailConstructorService ConstructNewEmailService<T>();
	}
}
