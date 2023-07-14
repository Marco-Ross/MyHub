using MyHub.Domain.Integration.AzureDevOps.AzureStorage;

namespace MyHub.Domain.Users.Interfaces
{
	public interface IUserGalleryService
	{
		Task<bool> RemoveUserImages(string userId);
		AzureStorageOptions GetGalleryImageStorageOptions(string fileName);
	}
}
