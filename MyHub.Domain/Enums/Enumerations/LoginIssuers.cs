namespace MyHub.Domain.Enums.Enumerations
{
	public class LoginIssuers : Enumeration
	{
		public static readonly LoginIssuers MarcosHub = new("", "");
		public static readonly LoginIssuers Google = new("G", "Google");
		public static readonly LoginIssuers Github = new("GH", "Github");

		protected LoginIssuers(string id, string name) : base(id, name)
		{
		}
	}
}
