using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Gallery
{
	public class GalleryImageComment
	{
		[Key]
		public string Id { get; set; } = string.Empty;

		[ForeignKey("GalleryImage")]
		public string ImageId { get; set; } = string.Empty;
		public GalleryImage GalleryImage { get; set; } = new GalleryImage();

		[ForeignKey("User")]
		public string? UserId { get; set; } = string.Empty;
		public User User { get; set; } = new User();

		public string Comment { get; set; } = string.Empty;
		public DateTime CommentDate { get; set; }

		public bool Pinned { get; set; }
		public DateTime? PinnedDate { get; set; }

		[ForeignKey("UserPinned")]
		public string? UserPinnedId { get; set; }
		public User? UserPinned { get; set; }
	}
}
