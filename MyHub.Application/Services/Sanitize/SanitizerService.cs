using Ganss.Xss;
using MyHub.Domain.Sanitizer.Interfaces;

namespace MyHub.Application.Services.Sanitize
{
	public class SanitizerService : ISanitizerService
	{
		private readonly HtmlSanitizer _htmlSanitizer;

		public SanitizerService()
		{
			_htmlSanitizer = new HtmlSanitizer();
		}

		public string Sanitize(string html, string? domain = null)
		{
			return string.IsNullOrWhiteSpace(domain) ? _htmlSanitizer.Sanitize(html) : _htmlSanitizer.Sanitize(html, domain);
		}
	}
}
