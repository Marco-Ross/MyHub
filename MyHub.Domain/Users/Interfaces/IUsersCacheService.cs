namespace MyHub.Domain.Users.Interfaces
{
	public interface IUsersCacheService
	{
		void RevokeAllUserLogins(AccessingUser user, Action<AccessingUser> func);
		void RevokeUserLoginsExceptCurrent(AccessingUser user, string currentRefreshToken, Action<AccessingUser, string> action);
		bool IsUserBlacklisted(string tokenId, string? userId);
	}
}
