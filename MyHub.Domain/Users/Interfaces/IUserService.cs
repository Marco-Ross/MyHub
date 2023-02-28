namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(User user);
		User? RevokeUser(string user);
		User? RevokeUser(User user);
		bool UserExists(string email);

		User? GetFullUserByEmail(string username);
		User? GetFullUserById(string id);
		void UpdateRefreshToken(User authenticatingUser, string refreshToken);
	}
}