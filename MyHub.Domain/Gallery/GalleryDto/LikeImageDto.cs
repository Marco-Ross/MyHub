using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Gallery.GalleryDto
{
	public class LikeImageDto
	{
		[Required]
		public string ImageId { get; set; } = string.Empty;
	}
}
