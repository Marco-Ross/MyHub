using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.AdminOptions;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Cache;

namespace MyHub.Application.Services.Users
{
	public class MarcoService : IMarcoService
	{
		private readonly IUsersService _usersService;
		private readonly IMemoryCache _memoryCache;
		private readonly MemoryCacheEntryOptions _cacheOptions;
		private readonly MarcoOptions _marco;


		public MarcoService(IUsersService usersService, IMemoryCache memoryCache, IOptions<MarcoOptions> marco)
		{
			_usersService = usersService;
			_memoryCache = memoryCache;
			_cacheOptions = new MemoryCacheEntryOptions { Priority = CacheItemPriority.NeverRemove };
			_marco = marco.Value;
		}
		private T? GetCache<T>(string key)
		{
			if (_memoryCache.TryGetValue<T>(key, out var cacheValue) && cacheValue is not null)
				return cacheValue;

			return cacheValue;
		}

		private T SetCache<T>(string key, T value)
		{
			return _memoryCache.Set(key, value, _cacheOptions);
		}

		public string GetMarcoId()
		{
			var cacheValue = GetCache<string>(CacheKeys.MarcoId);

			if (!string.IsNullOrWhiteSpace(cacheValue))
				return cacheValue;

			var user = _usersService.GetUserByEmail(_marco.Email) ?? new User();

			return SetCache(CacheKeys.MarcoId, user.Id);
		}
	}
}
