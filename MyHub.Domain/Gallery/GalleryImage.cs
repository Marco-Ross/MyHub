using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Gallery
{
	public class GalleryImage
	{
		[Key]
		public string Id { get; set; } = string.Empty;

		[ForeignKey("UserCreated")]
		public string? UserCreatedId { get; set; } = string.Empty;
		public User? UserCreated { get; set; }

		public string Caption { get; set; } = string.Empty;
		public DateTime DateUploaded { get; set; }
		public ICollection<GalleryImageComment> GalleryImageComments { get; set; } = new List<GalleryImageComment>();

		[InverseProperty("LikedGalleryImages")]
		public ICollection<User> LikedGalleryUsers { get; set; } = new List<User>();

		[NotMapped]
		public int LikesCount { get; set; }
		[NotMapped]
		public int CommentsCount { get; set; }

		[Timestamp]
		public byte[]? Version { get; set; }
	}
}
