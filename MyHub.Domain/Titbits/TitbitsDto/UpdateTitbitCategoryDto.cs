using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Titbits.TitbitsDto
{
	public class UpdateTitbitCategoryDto
	{
		[Required]
		public List<TitbitCategoryDto> Categories { get; set; } = new List<TitbitCategoryDto>();
	}
}
