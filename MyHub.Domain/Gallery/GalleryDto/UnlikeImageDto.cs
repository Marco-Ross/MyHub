using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Gallery.GalleryDto
{
	public class UnlikeImageDto
	{
		[Required]
		public string ImageId { get; set; } = string.Empty;
	}
}
