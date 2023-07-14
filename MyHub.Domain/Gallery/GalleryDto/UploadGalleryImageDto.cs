using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Gallery.GalleryDto
{
	public class UploadGalleryImageDto
	{
		[Required]
		public string Image { get; set; } = string.Empty;
		public string Caption { get; set; } = string.Empty;
	}
}
