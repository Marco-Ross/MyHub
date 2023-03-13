namespace MyHub.Domain.Emails.Interfaces
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(Email email);
	}
}
