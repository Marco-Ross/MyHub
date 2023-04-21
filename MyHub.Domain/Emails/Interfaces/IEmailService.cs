namespace MyHub.Domain.Emails.Interfaces
{
	public interface IEmailService
	{
		Task CreateAndSendEmail<T>(T email) where T : Email;
	}
}
