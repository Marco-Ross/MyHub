using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails.EmailTemplates
{
	public class PasswordRecoveryEmail : Email
	{
		public PasswordRecoveryEmail() => Template = "PasswordRecoveryTemplate.html";

		[NotMapped]
		public string ResetPasswordToken { get; set; } = string.Empty;
	}
}
