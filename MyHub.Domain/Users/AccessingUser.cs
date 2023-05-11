using MyHub.Domain.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHub.Domain.Users
{
	public class AccessingUser
    {
		[Key, ForeignKey("User")]
        public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PasswordSalt { get; set; } = string.Empty;
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
		public string RegisterToken { get; set; } = string.Empty;
		public string RegisterTokenSalt { get; set; } = string.Empty;
		public string ResetPasswordToken { get; set; } = string.Empty;
		public string ResetPasswordTokenSalt { get; set; } = string.Empty;
		public DateTime? RegisterTokenExpireDate { get; set; } = null;
		public DateTime? ResetPasswordTokenExpireDate { get; set; } = null;
		public DateTime? EmailVerificationDate { get; set; } = null;
		public DateTime? LastResetPasswordDate { get; set; } = null;
		public bool IsEmailVerified { get; set; }

		public User User { get; set; } = new User();

		[NotMapped]
		public string ProfileImage { get; set; } = string.Empty;
	}
}
