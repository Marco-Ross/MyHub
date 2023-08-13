using MyHub.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Titbits
{
	public class Titbit
	{
		[Key]
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;

		[ForeignKey("UserCreated")]
		public string? UserCreatedId { get; set; }
		public User? UserCreated { get; set; }

		public ICollection<TitbitLink> TitbitLinks { get; set; } = new List<TitbitLink>();

		[ForeignKey("TitbitCategory")]
		public string? CategoryId { get; set; } = string.Empty;
		public TitbitCategory? TitbitCategory { get; set; }
		public DateTime DateUploaded { get; set; }

		[ForeignKey("UserUpdated")]
		public string? UserUpdatedId { get; set; }
		public User? UserUpdated { get; set; }

		public DateTime DateUpdated { get; set; }

		[InverseProperty("LikedTitbits")]
		public ICollection<User> LikedTitbitUsers { get; set; } = new List<User>();
	}
}
