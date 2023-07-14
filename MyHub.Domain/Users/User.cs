using MyHub.Domain.Gallery;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Users
{
	public class User
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Theme { get; set; } = string.Empty;

		[InverseProperty("UserCreated")]
		public ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();

		[InverseProperty("LikedUsers")]
		public ICollection<GalleryImage> LikedImages { get; set; } = new List<GalleryImage>();
	}
}
