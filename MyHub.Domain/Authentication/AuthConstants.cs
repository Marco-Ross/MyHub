namespace MyHub.Domain.Authentication
{
	public static class AuthConstants
	{
		public static string AccessTokenHeader => "X-Access-Token";
		public static string RefreshTokenHeader => "X-Refresh-Token";
		public static string LoggedInHeader => "X-Logged-In";
		public static string ForgeryTokenHeader => "X-Forgery-Token";
		public static string ApiKeyHeader => "X-Api-Key";
	}
}
