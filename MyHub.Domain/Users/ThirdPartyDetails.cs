using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Users
{
	public class ThirdPartyDetails
	{
		[Key, ForeignKey("AccessingUser")]
		public string Id { get; set; } = string.Empty;
		public AccessingUser? AccessingUser { get; set; }

		public string? ThirdPartyIssuerId { get; set; } = string.Empty;
	}
}
