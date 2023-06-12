using Microsoft.Extensions.Caching.Memory;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Cache;

namespace MyHub.Application.Services.Users
{
	public class UsersCacheService : IUsersCacheService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly MemoryCacheEntryOptions _cacheOptions;

		public UsersCacheService(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
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

		public void RevokeAllUserLogins(AccessingUser user, Action<AccessingUser> action)
		{
			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + user.Id) ?? new List<string>();

			var blacklisted = user.RefreshTokens.Select(x => x.Token).ToList();

			action(user);

			cacheValue.AddRange(blacklisted);

			SetCache(CacheKeys.BlacklistedLogins + user.Id, cacheValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
		}
		
		public void RevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken, Action<AccessingUser, string> action)
		{
			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + user.Id) ?? new List<string>();

			var blacklisted = user.RefreshTokens.Where(x => x.Token != currentRefreshToken).Select(x => x.Token).ToList();

			action(user, currentRefreshToken);

			cacheValue.AddRange(blacklisted);

			SetCache(CacheKeys.BlacklistedLogins + user.Id, cacheValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
		}

		public bool IsUserBlacklisted(string tokenId, string? userId)
		{
			var cacheValue = GetCache<List<string>>(CacheKeys.BlacklistedLogins + userId) ?? new List<string>();

			return cacheValue.Contains(tokenId);
		}
	}
}
