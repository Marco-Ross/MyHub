namespace MyHub.Domain.ConfigurationOptions.Authentication
{
	public class ThirdPartyLogin
	{
		public Google Google { get; set; } = new Google();
		public Facebook Facebook{ get; set; } = new Facebook();
	}
}
