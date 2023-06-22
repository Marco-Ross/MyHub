namespace MyHub.Domain.Enums.Enumerations
{
	public class CustomJwtClaimNames : Enumeration
	{
		public static readonly CustomJwtClaimNames Picture = new("picture", "picture");
		public static readonly CustomJwtClaimNames IssuerManaging = new("issManaging", "issManaging");

		protected CustomJwtClaimNames(string id, string name) : base(id, name)
		{
		}
	}
}
