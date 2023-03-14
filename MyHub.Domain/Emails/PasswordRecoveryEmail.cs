using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails
{
    public class PasswordRecoveryEmail : Email
    {
        public PasswordRecoveryEmail() => TemplateName = "PasswordRecoveryTemplate.html";

        [NotMapped]
        public string ResetPasswordToken { get; set; } = string.Empty;
    }
}
