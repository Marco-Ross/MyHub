using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Titbits
{
	public class TitbitLink
	{
		[Key]
		public string Id { get; set; } = string.Empty;

		[ForeignKey("Titbit")]
		public string TitbitId { get; set; } = string.Empty;
		public Titbit Titbit { get; set; } = new Titbit();

		public string Title { get; set; } = string.Empty;
		public string Link { get; set; } = string.Empty;
	}
}
