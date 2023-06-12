using MyHub.Domain.Users;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Interfaces
{
    public interface IAuthenticationService : ISharedAuthService
	{
		Task<Validator> RegisterUser(AccessingUser accessingUser);
		Validator<LoginDetails> AuthenticateUser(string email, string password);
		bool RevokeUser(string userId, string refreshToken);
		Validator VerifyUserEmail(string userId, string token);
		Task<Validator> ResetPasswordEmail(string email);
		Validator ResetPasswordLoggedIn(string userId, string oldPassword, string newPassword, string refreshToken);
		Validator ResetPassword(string userId, string password, string resetPasswordToken);
		Task DeleteUser(string userId);
		string AuthenticateUserGetTokens(string userid, string email, string password);
		Task<Validator> ChangeUserEmail(string userId, string newEmail, string idToken);
		Validator ChangeUserEmailComplete(string userId, string changeEmailToken);
	}
}
