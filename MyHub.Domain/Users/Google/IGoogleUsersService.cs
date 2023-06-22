namespace MyHub.Domain.Users.Google
{
	public interface IGoogleUsersService
    {
		Task<Stream?> GetUserProfileImage(string pictureUrl);
	}
}
