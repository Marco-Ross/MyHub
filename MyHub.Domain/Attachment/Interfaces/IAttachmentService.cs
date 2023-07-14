using MyHub.Domain.Gallery;

namespace MyHub.Domain.Attachment.Interfaces
{
	public interface IAttachmentService
	{
		GalleryImage? AttachGalleryImageToUser(string userId, string fileName, string caption);
	}
}
