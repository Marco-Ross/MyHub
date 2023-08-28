namespace MyHub.Domain.Sanitizer.Interfaces
{
	public interface ISanitizerService
	{
		string Sanitize(string html, string? domain = default);
	}
}
