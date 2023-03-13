using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Users
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IEncryptionService _encryptionService;

		public UserService(ApplicationDbContext applicationDbContext, IEncryptionService encryptionService)
		{
			_applicationDbContext = applicationDbContext;
			_encryptionService = encryptionService;
		}

		public User RegisterUser(User user, string registerToken)
		{
			var hashedPassword = _encryptionService.HashData(user.Password, out var passwordSalt);
			var hashedRegisterToken = _encryptionService.HashData(registerToken, out var tokenSalt);

			user.Id = Guid.NewGuid().ToString();
			user.Password = hashedPassword;
			user.PasswordSalt = Convert.ToHexString(passwordSalt);
			user.RegisterToken = hashedRegisterToken;
			user.RegisterTokenSalt = Convert.ToHexString(tokenSalt);
			user.RegisterTokenExpireDate = DateTime.Now.AddHours(3);

			var savedUser = _applicationDbContext.Add(user);
			
			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public Validator VerifyUserRegistration(User user, string token)
		{
			if(string.IsNullOrWhiteSpace(user.RegisterToken))
				return new Validator().AddError("Email verification link invalid.");

			if (!_encryptionService.VerifyData(token, user.RegisterToken, user.RegisterTokenSalt))
				return new Validator().AddError("Cannot register user with invalid link.");
			
			if (user.RegisterTokenExpireDate < DateTime.Now)
				return new Validator().AddError("Email verification link has expired.");

			user.IsEmailVerified = true;
			user.RegisterToken = string.Empty;
			user.RegisterTokenSalt = string.Empty;
			user.RegisterTokenExpireDate = null;
			user.EmailVerificationDate = DateTime.Now;

			_applicationDbContext.SaveChanges();

			return new Validator();
		}

		public User ResetUserPassword(User user, string resetToken)
		{
			var hashedResetToken = _encryptionService.HashData(resetToken, out var tokenSalt);
			
			user.ResetPasswordToken = hashedResetToken;
			user.ResetPasswordTokenSalt = Convert.ToHexString(tokenSalt);
			user.ResetPasswordTokenExpireDate = DateTime.Now.AddHours(3);

			_applicationDbContext.SaveChanges();

			return user;
		}

		public Validator VerifyUserPasswordReset(User user, string password, string resetPasswordToken)
		{
			if (string.IsNullOrWhiteSpace(user.ResetPasswordToken))
				return new Validator().AddError("Reset password link invalid.");

			if (!_encryptionService.VerifyData(resetPasswordToken, user.ResetPasswordToken, user.ResetPasswordTokenSalt))
				return new Validator().AddError("Cannot register user with invalid link.");

			if (user.ResetPasswordTokenExpireDate < DateTime.Now)
				return new Validator().AddError("Reset password link has expired.");

			user.Password = _encryptionService.HashData(password, out var passwordSalt);
			user.PasswordSalt = Convert.ToHexString(passwordSalt);
			user.ResetPasswordToken = string.Empty;
			user.ResetPasswordTokenSalt = string.Empty;
			user.ResetPasswordTokenExpireDate = null;
			user.LastResetPasswordDate = DateTime.Now;

			_applicationDbContext.SaveChanges();

			return new Validator();
		}

		public User? RevokeUser(string userId) => RevokeUser(GetFullUserById(userId));

		public User? RevokeUser(User? user)
		{
			if (user == null)
				return null;

			user.RefreshToken = string.Empty;

			_applicationDbContext.SaveChanges();

			return user;
		}

		public bool UserExists(string email) => _applicationDbContext.Users.Any(x => x.Email == email);

		public User? GetFullUserByEmail(string email) => _applicationDbContext.Users.SingleOrDefault(x => x.Email == email);

		public User? GetFullUserById(string id) => _applicationDbContext.Find<User>(id);

		public void UpdateRefreshToken(User authenticatingUser, string refreshToken)
		{
			authenticatingUser.RefreshToken = refreshToken;

			_applicationDbContext.SaveChanges();
		}
	}
}
