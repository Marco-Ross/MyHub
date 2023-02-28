namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(User user);
		User? RevokeUser(string user);
		User? RevokeUser(User user);
		User? GetUser(string id);
		bool UserExists(string email);

		User? GetUserByEmail(string username);
		void UpdateRefreshToken(User authenticatingUser, string refreshToken);
	}
}