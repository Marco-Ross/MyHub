namespace MyHub.Domain.Enums.Enumerations
{
	public class LoginIssuers : Enumeration
	{
		public static readonly LoginIssuers MarcosHub = new("MH", "MarcosHub");
		public static readonly LoginIssuers Google = new("G", "Google");
		public static readonly LoginIssuers Facebook = new("F", "Facebook");

		protected LoginIssuers(string id, string name) : base(id, name)
		{
		}
	}
}
