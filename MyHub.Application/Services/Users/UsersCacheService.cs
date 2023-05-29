using Microsoft.Extensions.Caching.Memory;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using MyHub.Infrastructure.Cache;

namespace MyHub.Application.Services.Users
{
	public class UsersCacheService : IUsersService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IUsersService _userService;
		private readonly MemoryCacheEntryOptions _cacheOptions;

		public UsersCacheService(IMemoryCache memoryCache, IUsersService userService)
		{
			_memoryCache = memoryCache;
			_userService = userService;
			_cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) };
		}

		private T? GetCache<T>(string key)
		{
			if (_memoryCache.TryGetValue<T>(key, out var cacheValue) && cacheValue is not null)
				return cacheValue;

			return cacheValue;
		}

		private T SetCache<T>(string key, T value, MemoryCacheEntryOptions? cacheOptions = default)
		{
			if (cacheOptions is not null)
				return _memoryCache.Set(key, value, _cacheOptions);

			return _memoryCache.Set(key, value, cacheOptions);
		}

		public AccessingUser RegisterUserDetails(AccessingUser newUser, string registerToken)
		{
			return _userService.RegisterUserDetails(newUser, registerToken);
		}

		public AccessingUser? RevokeUser(string userId, string refreshToken)
		{
			return _userService.RevokeUser(userId, refreshToken);
		}

		public AccessingUser? RevokeUser(AccessingUser user, string refreshToken)
		{
			return _userService.RevokeUser(user, refreshToken);
		}

		public bool UserExists(string email)
		{
			return _userService.UserExists(email);
		}

		public AccessingUser? GetFullAccessingUserByEmail(string username)
		{
			return _userService.GetFullAccessingUserByEmail(username);
		}

		public AccessingUser? GetFullAccessingUserById(string id)
		{
			return _userService.GetFullAccessingUserById(id);
		}

		public void AddRefreshToken(AccessingUser authenticatingUser, string refreshToken)
		{
			_userService.AddRefreshToken(authenticatingUser, refreshToken);
		}

		public void UpdateRefreshToken(AccessingUser authenticatingUser, string oldRefreshToken, string refreshToken)
		{
			_userService.UpdateRefreshToken(authenticatingUser, oldRefreshToken, refreshToken);
		}

		public Validator VerifyUserRegistration(AccessingUser user, string token)
		{
			return _userService.VerifyUserRegistration(user, token);
		}

		public AccessingUser ResetUserPasswordLoggedIn(AccessingUser user, string newPassword)
		{
			return _userService.ResetUserPasswordLoggedIn(user, newPassword);
		}

		public AccessingUser ResetUserPassword(AccessingUser user, string resetToken)
		{
			return _userService.ResetUserPassword(user, resetToken);
		}

		public Validator VerifyUserPasswordReset(AccessingUser user, string password, string resetPasswordToken)
		{
			return _userService.VerifyUserPasswordReset(user, password, resetPasswordToken);
		}

		public async Task<bool> UploadUserProfileImage(AccessingUser user)
		{
			return await _userService.UploadUserProfileImage(user);
		}

		public async Task<bool> UpdateUserProfileImage(string userId, string image)
		{
			return await _userService.UpdateUserProfileImage(userId, image);
		}

		public async Task<Stream?> GetUserProfileImage(string userId)
		{
			return await _userService.GetUserProfileImage(userId);
		}

		public void UpdateUserTheme(string userId, string theme)
		{
			_userService.UpdateUserTheme(userId, theme);
		}

		public string GetUserTheme(string userId)
		{
			return _userService.GetUserTheme(userId);
		}

		public void UpdateUserAccount(AccessingUser accessingUser, string userId)
		{
			_userService.UpdateUserAccount(accessingUser, userId);
		}

		public async Task DeleteUser(string userId)
		{
			await _userService.DeleteUser(userId);
		}

		public async Task<bool> DeleteUserProfileImage(string userId)
		{
			return await _userService.DeleteUserProfileImage(userId);
		}

		public void UpdateUserEmail(AccessingUser user, string newEmail, string emailChangeToken)
		{
			_userService.UpdateUserEmail(user, newEmail, emailChangeToken);
		}

		public Validator ChangeUserEmailComplete(AccessingUser user, string changeEmailToken)
		{
			return _userService.ChangeUserEmailComplete(user, changeEmailToken);
		}

		public void RevokeAllUserLogins(AccessingUser user)
		{
			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + user.Id) ?? new List<string>();

			var blacklisted = user.RefreshTokens.Select(x => x.Token).ToList();

			_userService.RevokeAllUserLogins(user);

			cacheValue.AddRange(blacklisted);

			SetCache(CacheKeys.BlacklistedLogins + user.Id, cacheValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
		}
		
		public void RevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken)
		{
			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + user.Id) ?? new List<string>();

			var blacklisted = user.RefreshTokens.Where(x => x.Token != currentRefreshToken).Select(x => x.Token).ToList();

			_userService.RevokeUserLoginsExceptCurrent(user, currentRefreshToken);

			cacheValue.AddRange(blacklisted);

			SetCache(CacheKeys.BlacklistedLogins + user.Id, cacheValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
		}
	}
}
