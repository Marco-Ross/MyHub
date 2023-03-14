using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Emails
{
	public class EmailService : IEmailService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IEmailSenderService _emailSenderService;
		private readonly IEmailConstructorFactory _emailConstructorFactory;

		public EmailService(ApplicationDbContext applicationDbContext, IEmailSenderService emailSenderService, IEmailConstructorFactory emailConstructorFactory)
		{
			_applicationDbContext = applicationDbContext;
			_emailSenderService = emailSenderService;
			_emailConstructorFactory = emailConstructorFactory;
		}

		public async Task CreateAndSendEmail<T>(T email) where T : Email
		{
			var content = _emailConstructorFactory.ConstructNewEmailService<T>().ConstructEmail(email);

			await _emailSenderService.SendEmailAsync(email, content);

			SaveEmail(email);
		}

		private void SaveEmail(Email email)
		{
			_applicationDbContext.Add(email);
			_applicationDbContext.SaveChanges();
		}
	}
}
