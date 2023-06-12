namespace MyHub.Domain.Authentication
{
	public static class AuthConstants
	{
		public static string AccessToken => "X-Access-Token";
		public static string IdToken => "X-Id-Token";
		public static string RefreshToken => "X-Refresh-Token";
		public static string LoggedIn => "X-Logged-In";
		public static string ForgeryToken => "X-Forgery-Token";
		public static string ApiKeyHeader => "X-Api-Key";
	}
}
