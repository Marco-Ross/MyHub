using MyHub.Domain.Validation;

namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
		AccessingUser RegisterUser(string email, string username, string password, string registerToken);
		AccessingUser? RevokeUser(string userId, string refreshToken);
		AccessingUser? RevokeUser(AccessingUser user, string refreshToken);
		bool UserExists(string email);

		AccessingUser? GetFullAccessingUserByEmail(string username);
		AccessingUser? GetFullAccessingUserById(string id);
		void AddRefreshToken(AccessingUser authenticatingUser, string refreshToken);
		void UpdateRefreshToken(AccessingUser authenticatingUser, string oldRefreshToken, string refreshToken);
		Validator VerifyUserRegistration(AccessingUser user, string token);
		AccessingUser ResetUserPassword(AccessingUser user, string resetToken);
		Validator VerifyUserPasswordReset(AccessingUser user, string password, string resetPasswordToken);
	}
}