using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Titbits.TitbitsDto
{
	public class DeleteTitbitCategoryDto
	{
		[Required]
		public List<TitbitCategoryDto> Categories { get; set; } = new List<TitbitCategoryDto>();
	}
}
