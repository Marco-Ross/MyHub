using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails.EmailTemplates
{
	public class AccountRegisterEmail : Email
	{
		public AccountRegisterEmail() => Template = "AccountRegisterTemplate.html";

		[NotMapped]
		public string RegisterToken { get; set; } = string.Empty;
	}
}
