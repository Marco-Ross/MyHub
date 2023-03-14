using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyHub.Application.Services.Emails
{
	public class SendGridEmailService : IEmailSenderService
	{
		private readonly ILogger _logger;
		private readonly AuthEmailSenderOptions _options;
		public SendGridEmailService(ILogger<SendGridEmailService> logger, IOptions<AuthEmailSenderOptions> options)
		{
			_logger = logger;
			_options = options.Value;
		}

		public async Task SendEmailAsync(Email email, string content)
		{
			if (string.IsNullOrWhiteSpace(_options.SendGridKey))
				throw new Exception("No Email SendGridKey");

			await Execute(_options.SendGridKey, email, content);
		}

		private async Task Execute(string sendGridKey, Email email, string content)
		{
			var client = new SendGridClient(sendGridKey);
			var message = new SendGridMessage()
			{
				From = new EmailAddress(email.From, email.FromName),
				Subject = email.Subject,
				HtmlContent = content
			};

			message.AddTo(new EmailAddress(email.To));

			message.SetClickTracking(false, false);

			var response = await client.SendEmailAsync(message);

			_logger.LogInformation(response.IsSuccessStatusCode ? $"Email to {email.To} queued successfully." : $"Failed to send Email to {email.To}");
		}
	}
}
