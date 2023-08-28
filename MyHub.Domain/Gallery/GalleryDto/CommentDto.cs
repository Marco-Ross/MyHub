using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Gallery.GalleryDto
{
	public class CommentDto
	{
		[Required]
		public string ImageId { get; set; } = string.Empty;

		[Required, StringLength(500)]
		public string Comment { get; set; } = string.Empty;
	}
}
