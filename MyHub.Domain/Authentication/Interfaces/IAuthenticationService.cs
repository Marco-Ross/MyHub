using MyHub.Domain.Users;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IAuthenticationService
	{
		Task<Validator> RegisterUser(AccessingUser accessingUser);
		Validator<LoginDetails> AuthenticateUser(string email, string password);
		Validator<LoginDetails> RefreshUserAuthentication(string accessToken, string refreshToken);
		bool RevokeUser(string userId);
		Validator VerifyUserEmail(string userId, string token);
		Task<Validator> ResetPasswordEmail(string email);
		Validator ResetPassword(string userId, string password, string resetPasswordToken);
	}
}
