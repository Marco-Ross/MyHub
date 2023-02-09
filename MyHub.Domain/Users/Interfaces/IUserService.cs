namespace MyHub.Domain.Users.Interfaces
{
    public interface IUserService
    {
        User CreateUser(User user);
		User UpdateUser(User user);
		User? GetUser(string id);

		User? GetUserWithCredentials(string username, string password);

	}
}