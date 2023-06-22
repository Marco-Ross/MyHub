using MyHub.Domain.Authentication.Facebook;

namespace MyHub.Domain.Users.Facebook
{
	public interface IFacebookUsersService
	{
		Task<FacebookDetailsResponse?> GetUserDetails(string accessToken);
		Task<Stream?> GetUserProfileImage(string pictureUrl);
	}
}
