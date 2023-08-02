namespace MyHub.Domain.Titbits.TitbitsDto
{
	public class TitbitDto
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public bool IsLiked { get; set; }
		public TitbitCategoryDto TitbitCategory { get; set; } = new TitbitCategoryDto();
		public List<TitbitLinkDto> TitbitLinks { get; set; } = new List<TitbitLinkDto>();
		public DateTime DateUploaded { get; set; }
	}
}
