using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails
{
    public class AccountRegisterEmail : Email
    {
        public AccountRegisterEmail() => TemplateName = "AccountRegisterTemplate.html";

        [NotMapped]
        public string RegisterToken { get; set; } = string.Empty;
    }
}
