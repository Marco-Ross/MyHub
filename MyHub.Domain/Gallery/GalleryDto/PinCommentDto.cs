using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Gallery.GalleryDto
{
	public class PinCommentDto
	{
		[Required]
		public string CommentId { get; set; } = string.Empty;
	}
}
