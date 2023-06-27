namespace MyHub.Domain.ConfigurationOptions.Authentication
{
	public class ThirdPartyLogin
	{
		public Google Google { get; set; } = new Google();
		public GitHub Github{ get; set; } = new GitHub();
	}
}
