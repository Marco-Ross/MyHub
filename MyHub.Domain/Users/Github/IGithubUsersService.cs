namespace MyHub.Domain.Users.Github
{
	public interface IGithubUsersService
	{
		Task<Stream?> GetUserProfileImage(string pictureUrl);
	}
}
