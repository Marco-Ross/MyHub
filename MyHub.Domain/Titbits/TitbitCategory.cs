using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Titbits
{
	public class TitbitCategory
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime DateUploaded { get; set; }
		public ICollection<Titbit> Titbits { get; set; } = new List<Titbit>();
	}
}
