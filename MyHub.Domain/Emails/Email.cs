using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Emails
{
	public class Email
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }

		[ForeignKey("AccessingUser")]
		public string UserId { get; set; } = string.Empty;
		public AccessingUser? User { get; set; }

		public string To { get; set; } = string.Empty;
		public string ToName { get; set; } = string.Empty;
		public string From { get; set; } = "noreply@marcoshub.com";
		public string FromName { get; set; } = "Marco's Hub";
		public string Subject { get; set; } = string.Empty;
		public string Template { get; set; } = "DefaultEmailTemplate.html";

		[NotMapped]
		public string ClientDomainAddress { get; set; } = string.Empty;
	}
}
