using MyHub.Domain.Users;

namespace MyHub.Domain.Gallery.Interfaces
{
	public interface IGalleryService
	{
		Task<GalleryImage?> UploadImage(string userId, string image, string caption);
		Task<Stream?> GetUserImage(string userId, string imageId);
		Task<bool> RemoveUserImage(string imageId);
		bool LikeImage(string currentUserId, string imageId);
		bool UnlikeImage(string currentUserId, string imageId);
		IEnumerable<GalleryImage> GetUserImages(string userId);
		GalleryImageComment? PostCommentToImage(string currentUserId, string imageId, string comment);
		List<GalleryImageComment> GetImageComments(string imageId);
		GalleryImage? GetImageData(string imageId);
	}
}