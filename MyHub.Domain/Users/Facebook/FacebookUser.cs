﻿namespace MyHub.Domain.Users.Facebook
{
	public class FacebookUser
	{
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string IdToken { get; set; } = string.Empty;
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
