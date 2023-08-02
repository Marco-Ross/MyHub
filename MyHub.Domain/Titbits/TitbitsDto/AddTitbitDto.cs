using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Titbits.TitbitsDto
{
	public class AddTitbitDto
	{
		[Required]
		public string Title { get; set; } = string.Empty;
		[Required]
		public string Description { get; set; } = string.Empty;
		[Required]
		public string CategoryId { get; set; } = string.Empty;
		public List<TitbitLinkDto> TitbitLinks { get; set; } = new List<TitbitLinkDto>();
	}
}
