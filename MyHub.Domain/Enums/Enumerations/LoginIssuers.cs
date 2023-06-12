namespace MyHub.Domain.Enums.Enumerations
{
	public class LoginIssuers : Enumeration
	{
		public static readonly LoginIssuers Google = new("G", "Google");

		protected LoginIssuers(string id, string name) : base(id, name)
		{
		}
	}
}
