using MyHub.Domain.Validation;

namespace MyHub.Domain.Users.Interfaces
{
	public interface IUsersService
	{
		AccessingUser RegisterUserDetails(AccessingUser newUser, string registerToken);
		AccessingUser? RevokeUser(string userId, string refreshToken);
		AccessingUser? RevokeUser(AccessingUser user, string refreshToken);
		bool UserExists(string email);
		AccessingUser? GetFullAccessingUserByEmail(string username);
		AccessingUser? GetFullAccessingUserById(string userId);
		void AddRefreshToken(AccessingUser authenticatingUser, string refreshToken);
		void UpdateRefreshToken(AccessingUser authenticatingUser, string oldRefreshToken, string refreshToken);
		Validator VerifyUserRegistration(AccessingUser user, string token);
		AccessingUser ResetUserPassword(AccessingUser user, string resetToken);
		Validator VerifyUserPasswordReset(AccessingUser user, string password, string resetPasswordToken);
		Task<bool> RegisterUserProfileImage(AccessingUser user);
		Task<bool> UpdateUserProfileImage(string userId, string image);
		Task<bool> UpdateUserProfileImage(string userId, Stream? image);
		void UpdateUserTheme(string userId, string theme);
		string GetUserTheme(string userId);
		void UpdateUserAccount(AccessingUser accessingUser, string userId);
		Task DeleteUser(string userId);
		Task<bool> DeleteUserProfileImage(string userId);
		AccessingUser ResetUserPasswordLoggedIn(AccessingUser user, string newPassword);
		void UpdateUserEmail(AccessingUser user, string newEmail, string emailChangeToken);
		Validator ChangeUserEmailComplete(AccessingUser user, string changeEmailToken);
		void RevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken);
		void RevokeAllUserLogins(AccessingUser user);
		Task<Stream?> GetUserProfileImage(string userId);
		AccessingUser RegisterThirdParty(AccessingUser newUser);
	}
}
