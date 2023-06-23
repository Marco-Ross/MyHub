namespace MyHub.Domain.Enums.Enumerations
{
	public class HubThemes : Enumeration
	{
		public static readonly HubThemes SystemTheme = new("S", "system-theme");
		public static readonly HubThemes DarkTheme = new("D", "dark-theme");
		public static readonly HubThemes LightTheme = new("L", "light-theme");

		protected HubThemes(string id, string name) : base(id, name)
		{
		}
	}
}
