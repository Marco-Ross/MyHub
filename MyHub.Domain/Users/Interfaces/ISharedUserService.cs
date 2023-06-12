namespace MyHub.Domain.Users.Interfaces
{
	public interface ISharedUsersService
	{
		Task<Stream?> GetUserProfileImage(string userId);
		AccessingUser? GetFullAccessingUserById(string userId);
	}
}
