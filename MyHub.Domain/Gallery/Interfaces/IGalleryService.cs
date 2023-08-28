using MyHub.Domain.Users;
using MyHub.Domain.Validation;

namespace MyHub.Domain.Gallery.Interfaces
{
	public interface IGalleryService
	{
		Task<GalleryImage?> UploadImage(string userId, string image, string caption);
		Task<Stream?> GetUserImage(string userId, string imageId);
		Task<bool> RemoveUserImage(string imageId);
		bool LikeImage(string currentUserId, string imageId);
		bool UnlikeImage(string currentUserId, string imageId);
		IEnumerable<GalleryImage> GetDisplayUserImages(string userId, string currentUserId);
		GalleryImageComment? PostCommentToImage(string currentUserId, string imageId, string comment);
		GalleryImage? GetExpandedImageData(string imageId, string currentUserId);
		Validator RemoveComment(string userId, string commentId);
	}
}