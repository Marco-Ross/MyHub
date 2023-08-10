using MyHub.Domain.Gallery;
using MyHub.Domain.Titbits;
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

		[InverseProperty("UserCreated")]
		public ICollection<Titbit> Titbits { get; set; } = new List<Titbit>();

		[InverseProperty("UserUpdated")]
		public ICollection<Titbit> TitbitsUpdated { get; set; } = new List<Titbit>();

		[InverseProperty("LikedGalleryUsers")]
		public ICollection<GalleryImage> LikedGalleryImages { get; set; } = new List<GalleryImage>();

		[InverseProperty("LikedTitbitUsers")]
		public ICollection<Titbit> LikedTitbits { get; set; } = new List<Titbit>();
	}
}
