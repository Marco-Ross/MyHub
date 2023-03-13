using System.ComponentModel.DataAnnotations;

namespace MyHub.Domain.Users
{
	public class User
    {
		[Key]
        public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PasswordSalt { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public string RegisterToken { get; set; } = string.Empty;
		public string RegisterTokenSalt { get; set; } = string.Empty;
		public string ResetPasswordToken { get; set; } = string.Empty;
		public string ResetPasswordTokenSalt { get; set; } = string.Empty;
		public DateTime? RegisterTokenExpireDate { get; set; } = null;
		public DateTime? ResetPasswordTokenExpireDate { get; set; } = null;
		public DateTime? EmailVerificationDate { get; set; } = null;
		public DateTime? LastResetPasswordDate { get; set; } = null;
		public bool IsEmailVerified { get; set; }
	}
}
