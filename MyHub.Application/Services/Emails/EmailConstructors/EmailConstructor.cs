using MyHub.Domain.Emails;

namespace MyHub.Application.Services.Emails.EmailConstructors
{
	public class EmailConstructor
	{
		private static string TemplatesFilePath => AppDomain.CurrentDomain.BaseDirectory;

		public static string ReadAllText(string templateFile) => File.ReadAllText(Path.Combine(TemplatesFilePath, "Emails", "EmailTemplates", templateFile));

		public static string AddDefaultEmailLayout(Email email, string innerTemplate)
		{
			var template = File.ReadAllText("DefaultEmailTemplate.html");

			template = template.Replace("{recipientName}", email.ToName);
			template = template.Replace("{senderName}", email.FromName);
			template = template.Replace("{subject}", email.Subject);
			template = template.Replace("{innerTemplate}", innerTemplate);

			return template;
		}
	}
}
