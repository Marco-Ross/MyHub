using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyHub.Application.Extensions;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Images.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using MyHub.Infrastructure.Cache;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Users
{
	public class UsersService : IUsersService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IEncryptionService _encryptionService;
		private readonly IAzureStorageService _azureStorageService;
		private readonly IUsersCacheService _usersCacheService;
		private readonly IUserGalleryService _userGalleryService;
		private readonly IMemoryCache _memoryCache;
		private readonly IImageService _imageService;

		public UsersService(ApplicationDbContext applicationDbContext, IEncryptionService encryptionService,
			IAzureStorageService azureStorageService, IUsersCacheService usersCacheService, IUserGalleryService userGalleryService,
			IMemoryCache memoryCache, IImageService imageService)
		{
			_applicationDbContext = applicationDbContext;
			_encryptionService = encryptionService;
			_azureStorageService = azureStorageService;
			_usersCacheService = usersCacheService;
			_usersCacheService = usersCacheService;
			_userGalleryService = userGalleryService;
			_memoryCache = memoryCache;
			_imageService = imageService;
		}

		public AccessingUser RegisterUserDetails(AccessingUser newUser, string registerToken)
		{
			var hashedPassword = _encryptionService.HashData(newUser.Password, out var passwordSalt);
			var hashedRegisterToken = _encryptionService.HashData(registerToken, out var tokenSalt);

			newUser.User.Id = Guid.NewGuid().ToString();
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

		public AccessingUser RegisterThirdParty(AccessingUser newUser)
		{
			newUser.IsEmailVerified = true;
			newUser.EmailVerificationDate = DateTime.Now;

			var savedUser = _applicationDbContext.AccessingUsers.Add(newUser);

			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public AccessingUser ResetUserPasswordLoggedIn(AccessingUser user, string newPassword)
		{
			user.Password = _encryptionService.HashData(newPassword, out var passwordSalt);
			user.PasswordSalt = Convert.ToHexString(passwordSalt);

			_applicationDbContext.SaveChanges();

			return user;
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

		public void RevokeAllUserLogins(AccessingUser user)
		{
			_usersCacheService.RevokeAllUserLogins(user, PerformRevokeAllUserLogins);
		}

		private void PerformRevokeAllUserLogins(AccessingUser user)
		{
			if (user is null)
				return;

			user.RefreshTokens = new List<RefreshToken>();

			_applicationDbContext.SaveChanges();
		}

		public void RevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken)
		{
			_usersCacheService.RevokeUserLoginsExceptCurrent(user, currentRefreshToken, PerformRevokeUserLoginsExceptCurrent);
		}

		private void PerformRevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken)
		{
			if (user is null)
				return;

			user.RefreshTokens = user.RefreshTokens.Where(x => x.Token == currentRefreshToken).ToList();

			_applicationDbContext.SaveChanges();
		}

		public bool UserExistsEmail(string email) => _applicationDbContext.Users.Any(x => x.Email == email);
		public bool UserExistsId(string id) => _applicationDbContext.Users.Any(x => x.Id == id);

		public AccessingUser? GetFullAccessingUserByEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email)) return null;
			return _applicationDbContext.AccessingUsers.Include(x => x.User).Include(x => x.RefreshTokens).Include(x => x.ThirdPartyDetails).SingleOrDefault(x => x.User.Email == email);
		}

		public AccessingUser? GetFullAccessingUserById(string id) => _applicationDbContext.AccessingUsers.Include(x => x.User).Include(x => x.RefreshTokens).Include(x => x.ThirdPartyDetails).SingleOrDefault(x => x.Id == id);
		public User? GetUserById(string id) => _applicationDbContext.Users.Include(x => x.GalleryImages).Include(x => x.LikedGalleryImages).SingleOrDefault(x => x.Id == id);
		public User? GetUserByEmail(string email) => _applicationDbContext.Users.Include(x => x.GalleryImages).Include(x => x.LikedGalleryImages).SingleOrDefault(x => x.Email == email);

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

		public async Task<bool> RegisterUserProfileImage(AccessingUser user)
		{
			if (string.IsNullOrWhiteSpace(user.ProfileImage))
				return false;

			return await UpdateUserProfileImage(user.Id, user.ProfileImage);
		}

		public async Task<bool> UpdateUserProfileImage(string userId, string image)
			=> await _azureStorageService.UploadFileToStorage(_imageService.CompressPng(image.ToMemoryStream()), GetProfileImageStorageOptions(userId));

		public async Task<bool> UpdateUserProfileImage(string userId, Stream? image)
			=> await _azureStorageService.UploadFileToStorage(image is not null ? _imageService.CompressPng(image) : image, GetProfileImageStorageOptions(userId));

		public async Task<Stream?> GetUserProfileImage(string userId)
			=> await _azureStorageService.GetFileFromStorage(GetProfileImageStorageOptions(userId));

		public async Task<bool> DeleteUserProfileImage(string userId)
			=> await _azureStorageService.RemoveFile(GetProfileImageStorageOptions(userId));

		private static string GetUserProfileImageName(string userId) => userId.AsPng();

		private static AzureStorageOptions GetProfileImageStorageOptions(string userId) => new()
		{
			StorageFolder = StorageFolder.ProfileImages,
			FileName = GetUserProfileImageName(userId),
			OverWrite = true
		};

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

		public void UpdateUserAccount(AccessingUser accessingUser, string userId)
		{
			var user = GetFullAccessingUserById(userId);

			if (user is null)
				return;

			user.User.Username = accessingUser.User.Username;
			user.User.Name = accessingUser.User.Name;
			user.User.Surname = accessingUser.User.Surname;

			_applicationDbContext.SaveChanges();
		}

		public async Task DeleteUser(string userId)
		{
			var user = GetUserById(userId);

			if (user is null) return;

			await _userGalleryService.RemoveUserImages(userId);

			var userToRemove = _applicationDbContext.Users.Include(x => x.Titbits).Include(x => x.TitbitsUpdated).Single(x=> x.Id == userId);

			_applicationDbContext.Users.Remove(userToRemove);

			await DeleteUserProfileImage(userId);

			_memoryCache.Remove(CacheKeys.MarcoId);

			_applicationDbContext.SaveChanges();
		}

		public void UpdateUserEmail(AccessingUser user, string newEmail, string changeEmailToken)
		{
			if (user is null) throw new ArgumentNullException("User", "User cannot be null.");

			var hashedEmailChangeToken = _encryptionService.HashData(changeEmailToken, out var tokenSalt);

			user.ChangeEmailToken = hashedEmailChangeToken;
			user.ChangeEmailTokenSalt = Convert.ToHexString(tokenSalt);
			user.ChangeEmailTokenExpireDate = DateTime.Now.AddHours(1);
			user.TemporaryNewEmail = newEmail;

			_applicationDbContext.SaveChanges();
		}

		public Validator ChangeUserEmailComplete(AccessingUser user, string changeEmailToken)
		{
			if (string.IsNullOrWhiteSpace(user.ChangeEmailToken))
				return new Validator().AddError("Change email address link invalid.");

			if (!_encryptionService.VerifyData(changeEmailToken, user.ChangeEmailToken, user.ChangeEmailTokenSalt))
				return new Validator().AddError("Cannot change email address with invalid link.");

			if (user.ChangeEmailTokenExpireDate is null || user.ChangeEmailTokenExpireDate < DateTime.Now)
				return new Validator().AddError("Change email address link has expired.");

			user.User.Email = user.TemporaryNewEmail;
			user.TemporaryNewEmail = string.Empty;
			user.ChangeEmailToken = string.Empty;
			user.ChangeEmailTokenSalt = string.Empty;
			user.ChangeEmailTokenExpireDate = null;

			_applicationDbContext.SaveChanges();

			return new Validator();
		}
	}
}
