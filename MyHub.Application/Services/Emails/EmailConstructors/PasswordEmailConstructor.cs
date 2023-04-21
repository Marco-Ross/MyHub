using MyHub.Domain.Emails;

namespace MyHub.Application.Services.Emails.EmailConstructors
{
    public class PasswordEmailConstructor : EmailConstructor, IEmailConstructorService
	{
		public string ConstructEmail<T>(T baseEmail) where T : Email
		{
			if (baseEmail is not PasswordRecoveryEmail email)
				throw new ArgumentException("Email cannot be null.");

			var constructedTemplate = ReadAllText(email.TemplateName);

			constructedTemplate = constructedTemplate.Replace("{UserId}", email.UserId);
			constructedTemplate = constructedTemplate.Replace("{ResetPasswordToken}", email.ResetPasswordToken);
			constructedTemplate = constructedTemplate.Replace("{ClientDomainAddress}", email.ClientDomainAddress);

			return constructedTemplate;
		}
	}
}
