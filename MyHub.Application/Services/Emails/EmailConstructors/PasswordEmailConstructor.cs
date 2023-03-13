using MyHub.Domain.Emails;
using MyHub.Domain.Emails.EmailTemplates;

namespace MyHub.Application.Services.Emails.EmailConstructors
{
	public class PasswordEmailConstructor : EmailConstructor, IEmailConstructorService
	{
		public Email ConstructEmail<T>(T baseEmail) where T : Email
		{
			if (baseEmail is not PasswordRecoveryEmail email)
				throw new ArgumentException("Email cannot be null.");

			email.Template = ReadAllText(email.Template);

			email.Template = email.Template.Replace("{UserId}", email.UserId);
			email.Template = email.Template.Replace("{ResetPasswordToken}", email.ResetPasswordToken);
			email.Template = email.Template.Replace("{ClientDomainAddress}", email.ClientDomainAddress);

			return email;
		}
	}
}
