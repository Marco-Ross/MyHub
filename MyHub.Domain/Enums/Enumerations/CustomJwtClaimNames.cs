namespace MyHub.Domain.Enums.Enumerations
{
	public class CustomJwtClaimNames : Enumeration
	{
		public static readonly CustomJwtClaimNames Picture = new("pic", "picture");

		protected CustomJwtClaimNames(string id, string name) : base(id, name)
		{
		}
	}
}
