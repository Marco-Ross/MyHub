namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
        User CreateUser(User user);
		User? RevokeUser(string user);
		User? RevokeUser(User user);
		User? GetUser(string id);

		User? GetUserWithCredentials(string username, string password);
		void UpdateRefreshToken(User authenticatingUser, string refreshToken);
	}
}