using MyHub.Domain.Emails;

namespace MyHub.Application.Services.Emails
{
	public interface IEmailConstructorService
	{
		public string ConstructEmail<T>(T email) where T : Email;
	}
}
