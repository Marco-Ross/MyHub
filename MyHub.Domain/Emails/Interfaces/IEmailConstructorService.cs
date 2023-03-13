using MyHub.Domain.Emails;

namespace MyHub.Application.Services.Emails
{
	public interface IEmailConstructorService
	{
		public Email ConstructEmail<T>(T email) where T : Email;
	}
}
