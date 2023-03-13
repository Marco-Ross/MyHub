using MyHub.Domain.Validation;

namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(User user, string registerToken);
		User? RevokeUser(string user);
		User? RevokeUser(User user);
		bool UserExists(string email);

		User? GetFullUserByEmail(string username);
		User? GetFullUserById(string id);
		void UpdateRefreshToken(User authenticatingUser, string refreshToken);
		Validator VerifyUserRegistration(User user, string token);
		User ResetUserPassword(User user, string resetToken);
		Validator VerifyUserPasswordReset(User user, string password, string resetPasswordToken);
	}
}