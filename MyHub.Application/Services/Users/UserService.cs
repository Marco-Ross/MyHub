using Microsoft.EntityFrameworkCore;
using MyHub.Application.Extensions;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
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
		private readonly IAzureStorageService _azureStorageService;

		public UserService(ApplicationDbContext applicationDbContext, IEncryptionService encryptionService, IAzureStorageService azureStorageService)
		{
			_applicationDbContext = applicationDbContext;
			_encryptionService = encryptionService;
			_azureStorageService = azureStorageService;
		}

		public AccessingUser RegisterUserDetails(AccessingUser newUser, string registerToken)
		{
			var hashedPassword = _encryptionService.HashData(newUser.Password, out var passwordSalt);
			var hashedRegisterToken = _encryptionService.HashData(registerToken, out var tokenSalt);

			newUser.User.Id =  Guid.NewGuid().ToString();
			newUser.Password = hashedPassword;
			newUser.PasswordSalt = Convert.ToHexString(passwordSalt);
			newUser.RegisterToken = hashedRegisterToken;
			newUser.RegisterTokenSalt = Convert.ToHexString(tokenSalt);
			newUser.RegisterTokenExpireDate = DateTime.Now.AddHours(3);
	
			var savedUser = _applicationDbContext.AccessingUsers.Add(newUser);

			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public Validator VerifyUserRegistration(AccessingUser user, string token)
		{
			if (string.IsNullOrWhiteSpace(user.RegisterToken))
				return new Validator().AddError("Email verification link invalid.");

			if (!_encryptionService.VerifyData(token, user.RegisterToken, user.RegisterTokenSalt))
				return new Validator().AddError("Cannot register user with invalid link.");

			if (user.RegisterTokenExpireDate is null || user.RegisterTokenExpireDate < DateTime.Now)//use an IClock and inject date for tests.
				return new Validator().AddError("Email verification link has expired.");

			user.IsEmailVerified = true;
			user.RegisterToken = string.Empty;
			user.RegisterTokenSalt = string.Empty;
			user.RegisterTokenExpireDate = null;
			user.EmailVerificationDate = DateTime.Now;

			_applicationDbContext.SaveChanges();

			return new Validator();
		}

		public AccessingUser ResetUserPassword(AccessingUser user, string resetToken)
		{
			var hashedResetToken = _encryptionService.HashData(resetToken, out var tokenSalt);

			user.ResetPasswordToken = hashedResetToken;
			user.ResetPasswordTokenSalt = Convert.ToHexString(tokenSalt);
			user.ResetPasswordTokenExpireDate = DateTime.Now.AddHours(3);

			_applicationDbContext.SaveChanges();

			return user;
		}

		public Validator VerifyUserPasswordReset(AccessingUser user, string password, string resetPasswordToken)
		{
			if (string.IsNullOrWhiteSpace(user.ResetPasswordToken))
				return new Validator().AddError("Reset password link invalid.");

			if (!_encryptionService.VerifyData(resetPasswordToken, user.ResetPasswordToken, user.ResetPasswordTokenSalt))
				return new Validator().AddError("Cannot register user with invalid link.");

			if (user.ResetPasswordTokenExpireDate is null || user.ResetPasswordTokenExpireDate < DateTime.Now)
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

		public AccessingUser? RevokeUser(string userId, string refreshToken) => RevokeUser(GetFullAccessingUserById(userId), refreshToken);

		public AccessingUser? RevokeUser(AccessingUser? user, string currentRefreshToken)
		{
			if (user is null)
				return null;

			if (string.IsNullOrWhiteSpace(currentRefreshToken))
				return null;

			var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == currentRefreshToken);

			if (refreshToken is null)
				user.RefreshTokens = new List<RefreshToken>();
			else
				user.RefreshTokens.Remove(refreshToken);

			_applicationDbContext.SaveChanges();

			return user;
		}

		public bool UserExists(string email) => _applicationDbContext.AccessingUsers.Any(x => x.Email == email);

		public AccessingUser? GetFullAccessingUserByEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email)) return null;
			return _applicationDbContext.AccessingUsers.Include(x => x.User).Include(x => x.RefreshTokens).SingleOrDefault(x => x.Email == email);
		}

		public AccessingUser? GetFullAccessingUserById(string id) => _applicationDbContext.AccessingUsers.Include(x => x.User).Include(x => x.RefreshTokens).SingleOrDefault(x => x.Id == id);
		public User? GetUserById(string id) => _applicationDbContext.Users.SingleOrDefault(x => x.Id == id);

		public void AddRefreshToken(AccessingUser authenticatingUser, string refreshToken)
		{
			authenticatingUser.RefreshTokens.Add(new RefreshToken { Id = Guid.NewGuid().ToString(), Token = refreshToken, CreatedDate = DateTime.Now });

			_applicationDbContext.SaveChanges();
		}

		public void UpdateRefreshToken(AccessingUser authenticatingUser, string oldRefreshToken, string refreshToken)
		{
			var refreshTokenToUpdate = authenticatingUser.RefreshTokens.FirstOrDefault(x => x.Token == oldRefreshToken);

			if (refreshTokenToUpdate is null)
				return;

			refreshTokenToUpdate.Token = refreshToken;
			refreshTokenToUpdate.CreatedDate = DateTime.Now;

			_applicationDbContext.SaveChanges();
		}

		public void UpdateUserTheme(string userId, string theme)
		{
			var user = GetUserById(userId);

			if (user is null) return;

			user.Theme = theme;

			_applicationDbContext.SaveChanges();
		}

		public string GetUserTheme(string userId)
		{
			var user = GetUserById(userId);

			if (user is null) return string.Empty;

			return user.Theme;
		}

		public async Task<bool> UploadUserProfileImage(AccessingUser user)
		{
			var profileImage = user.ProfileImage[(user.ProfileImage.LastIndexOf(',') + 1)..];

			return await _azureStorageService.UploadFileToStorage(StorageFolder.ProfileImages, profileImage.ToMemoryStream(), $"{user.Id}.png");
		}

		public async Task<Stream?> GetUserProfileImage(string userId)
		{
			return await _azureStorageService.GetFileFromStorage(StorageFolder.ProfileImages, $"{userId}.png");
		}
	}
}
