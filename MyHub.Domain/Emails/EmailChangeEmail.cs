using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails
{
	public class EmailChangeEmail : Email
	{
		public EmailChangeEmail() => TemplateName = "ChangeEmailTemplate.html";

		[NotMapped]
		public string ChangeEmailToken { get; set; } = string.Empty;
		[NotMapped]
		public string PreviousEmail { get; set; } = string.Empty;
	}
}
