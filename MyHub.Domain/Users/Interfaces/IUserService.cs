using MyHub.Domain.Validation;

namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
		AccessingUser RegisterUser(string email, string username, string password, string registerToken);
		AccessingUser? RevokeUser(string user);
		AccessingUser? RevokeUser(AccessingUser user);
		bool UserExists(string email);

		AccessingUser? GetFullAccessingUserByEmail(string username);
		AccessingUser? GetFullAccessingUserById(string id);
		void UpdateRefreshToken(AccessingUser authenticatingUser, string refreshToken);
		Validator VerifyUserRegistration(AccessingUser user, string token);
		AccessingUser ResetUserPassword(AccessingUser user, string resetToken);
		Validator VerifyUserPasswordReset(AccessingUser user, string password, string resetPasswordToken);
	}
}