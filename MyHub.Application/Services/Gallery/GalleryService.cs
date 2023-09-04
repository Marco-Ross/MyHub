using Microsoft.EntityFrameworkCore;
using MyHub.Application.Extensions;
using MyHub.Domain.Attachment.Interfaces;
using MyHub.Domain.Gallery;
using MyHub.Domain.Gallery.Interfaces;
using MyHub.Domain.Images.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Gallery
{
	public class GalleryService : IGalleryService
	{
		private readonly IAzureStorageService _azureStorageService;
		private readonly IAttachmentService _attachmentService;
		private readonly IUsersService _usersService;
		private readonly IUserGalleryService _userGalleryService;
		private readonly IImageService _imageService;
		private readonly ApplicationDbContext _applicationDbContext;

		public GalleryService(IAzureStorageService azureStorageService, IAttachmentService attachmentService, IUsersService usersService,
			ApplicationDbContext applicationDbContext, IUserGalleryService userGalleryService, IImageService imageService)
		{
			_azureStorageService = azureStorageService;
			_attachmentService = attachmentService;
			_usersService = usersService;
			_applicationDbContext = applicationDbContext;
			_userGalleryService = userGalleryService;
			_imageService = imageService;
		}

		public async Task<GalleryImage?> UploadImage(string userId, string image, string caption)
		{
			var fileName = Guid.NewGuid().ToString();

			var imageUploaded = await _azureStorageService.UploadFileToStorage(_imageService.CompressPng(image.ToMemoryStream()), _userGalleryService.GetGalleryImageStorageOptions(fileName));

			if (!imageUploaded)
				return null;

			return _attachmentService.AttachGalleryImageToUser(userId, fileName, caption);
		}

		public async Task<Stream?> GetUserImage(string userId, string imageId)
		{
			var user = _usersService.GetUserById(userId);

			if (user is null)
				return null;

			return await _azureStorageService.GetFileFromStorage(_userGalleryService.GetGalleryImageStorageOptions(imageId));
		}

		public async Task<bool> RemoveUserImage(string imageId)
		{
			var fileRemoved = await _azureStorageService.RemoveFile(_userGalleryService.GetGalleryImageStorageOptions(imageId));

			if (!fileRemoved)
				return false;

			var galleryImage = _applicationDbContext.GalleryImages.First(x => x.Id == imageId);

			_applicationDbContext.GalleryImages.Remove(galleryImage);

			_applicationDbContext.SaveChanges();

			return true;
		}

		public async Task<bool> RemoveUserImages(string userId) => await _userGalleryService.RemoveUserImages(userId);

		public bool LikeImage(string currentUserId, string imageId)
		{
			try
			{
				var currentUser = _usersService.GetUserById(currentUserId);

				if (currentUser is null)
					return false;

				var galleryImage = _applicationDbContext.GalleryImages.SingleOrDefault(x => x.Id == imageId);

				if (galleryImage is null) return false;

				galleryImage.LikedGalleryUsers.Add(currentUser);

				_applicationDbContext.SaveChanges();

				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}

		public bool UnlikeImage(string currentUserId, string imageId)
		{
			try
			{
				var currentUser = _usersService.GetUserById(currentUserId);

				if (currentUser is null)
					return false;

				var galleryImage = _applicationDbContext.GalleryImages.SingleOrDefault(x => x.Id == imageId);

				if (galleryImage is null) return false;

				galleryImage.LikedGalleryUsers.Remove(currentUser);

				_applicationDbContext.SaveChanges();

				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}

		public IEnumerable<GalleryImage> GetDisplayUserImages(string userId, string currentUserId)
		{
			return _applicationDbContext.GalleryImages.Include(x => x.LikedGalleryUsers).Include(x => x.GalleryImageComments).OrderByDescending(x => x.DateUploaded).Where(x => x.UserCreatedId == userId).Select(x => new GalleryImage
			{
				Id = x.Id,
				CommentsCount = x.GalleryImageComments.Count,
				LikesCount = x.LikedGalleryUsers.Count,
				LikedGalleryUsers = GetLikedUser(currentUserId, x),
				Caption = x.Caption,
				DateUploaded = x.DateUploaded,
				UserCreatedId = x.UserCreatedId
			});
		}

		public GalleryImage? GetExpandedImageData(string imageId, string currentUserId)
		{
			return _applicationDbContext.GalleryImages.Include(x => x.LikedGalleryUsers).Include(x => x.GalleryImageComments).ThenInclude(c => c.User).Select(x => new GalleryImage
			{
				Id = x.Id,
				CommentsCount = x.GalleryImageComments.Count,
				LikesCount = x.LikedGalleryUsers.Count,
				LikedGalleryUsers = GetLikedUser(currentUserId, x),
				GalleryImageComments = x.GalleryImageComments.OrderByDescending(x => x.PinnedDate).ThenByDescending(x => x.CommentDate).ToList(),
				Caption = x.Caption,
				DateUploaded = x.DateUploaded,
				UserCreatedId = x.UserCreatedId
			}).SingleOrDefault(x => x.Id == imageId);
		}

		private static List<User> GetLikedUser(string currentUserId, GalleryImage image)
		{
			var likedCurrentUser = image.LikedGalleryUsers.Where(x => x.Id == currentUserId).ToList();

			var likedUser = image.LikedGalleryUsers.FirstOrDefault(x => x.Id != currentUserId);

			if (likedUser is not null)
				likedCurrentUser.Add(likedUser);

			return likedCurrentUser;
		}

		public GalleryImageComment? PostCommentToImage(string currentUserId, string imageId, string comment)
		{
			if (string.IsNullOrWhiteSpace(comment))
				return null;

			var currentUser = _usersService.GetUserById(currentUserId);

			if (currentUser is null)
				return null;

			var galleryImage = _applicationDbContext.GalleryImages.SingleOrDefault(x => x.Id == imageId);

			if (galleryImage is null)
				return null;

			var newComment = AddComment(currentUser, comment);

			galleryImage.GalleryImageComments.Add(newComment);

			_applicationDbContext.SaveChanges();

			return newComment;
		}

		private static GalleryImageComment AddComment(User currentUser, string comment)
		{
			return new GalleryImageComment
			{
				Id = Guid.NewGuid().ToString(),
				User = currentUser,
				Comment = comment,
				CommentDate = DateTime.Now
			};
		}

		public Validator RemoveComment(string userId, string commentId)
		{
			var comment = _applicationDbContext.GalleryImageComments.SingleOrDefault(x => x.Id == commentId);

			if (comment is null) return new Validator().AddError("Comment does not exist.");

			if (comment.UserId != userId) return new Validator().AddError("Comment does not belong to user.");

			_applicationDbContext.GalleryImageComments.Remove(comment);

			_applicationDbContext.SaveChanges();

			return new Validator();
		}

		public void PinComment(string userId, string commentId)
		{
			var comment = _applicationDbContext.GalleryImageComments.Single(x => x.Id == commentId);

			var userPinned = _applicationDbContext.Users.Single(x => x.Id == userId);

			comment.Pinned = true;
			comment.PinnedDate = DateTime.Now;
			comment.UserPinned = userPinned;

			_applicationDbContext.SaveChanges();
		}

		public void UnpinComment(string userId, string commentId)
		{
			var comment = _applicationDbContext.GalleryImageComments.Single(x => x.Id == commentId);

			comment.Pinned = false;
			comment.PinnedDate = null;
			comment.UserPinnedId = null;
			comment.UserPinned = null;

			_applicationDbContext.SaveChanges();
		}
	}
}
