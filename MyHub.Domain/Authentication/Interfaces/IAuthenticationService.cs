using MyHub.Domain.Validation;

namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IAuthenticationService
	{
		Task<Validator> RegisterUser(string email, string username, string password);
		Validator<LoginDetails> AuthenticateUser(string username, string password);
		Validator<LoginDetails> RefreshUserAuthentication(string accessToken, string refreshToken);
		bool RevokeUser(string userId);
		Validator VerifyUserEmail(string userId, string token);
		Task<Validator> ResetPasswordEmail(string email);
		Validator ResetPassword(string userId, string password, string resetPasswordToken);
	}
}
