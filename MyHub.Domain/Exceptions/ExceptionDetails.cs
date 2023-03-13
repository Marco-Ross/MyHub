using System.Text.Json;

namespace MyHub.Domain.Exceptions
{
	public class ExceptionDetails
	{
		public int StatusCode { get; set; }
		public string? Message { get; set; }
		public string? InnerMessage { get; set; }

		public override string ToString()
		{
			return JsonSerializer.Serialize(this);
		}
	}
}
