using MyHub.Domain.Emails;

namespace MyHub.Application.Services.Emails.EmailConstructors
{
	public class ChangeEmailConstructor : EmailConstructor, IEmailConstructorService
	{
		public string ConstructEmail<T>(T baseEmail) where T : Email
		{
			if (baseEmail is not EmailChangeEmail email)
				throw new ArgumentException("Email cannot be null.");

			var constructedTemplate = ReadAllText(email.TemplateName);

			constructedTemplate = constructedTemplate.Replace("{UserId}", email.UserId);
			constructedTemplate = constructedTemplate.Replace("{ChangeEmailToken}", email.ChangeEmailToken);
			constructedTemplate = constructedTemplate.Replace("{ClientDomainAddress}", email.ClientDomainAddress);
			constructedTemplate = constructedTemplate.Replace("{PreviousEmail}", email.PreviousEmail);

			return constructedTemplate;
		}
	}
}
