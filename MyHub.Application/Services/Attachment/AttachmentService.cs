using MyHub.Domain.Attachment.Interfaces;
using MyHub.Domain.Gallery;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Attachment
{
	public class AttachmentService : IAttachmentService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IUsersService _usersService;

		public AttachmentService(ApplicationDbContext applicationDbContext, IUsersService usersService)
		{
			_applicationDbContext = applicationDbContext;
			_usersService = usersService;
		}

		public GalleryImage? AttachGalleryImageToUser(string userId, string fileName, string caption)
		{
			var user = _usersService.GetUserById(userId);

			if (user is null)
				return null;

			var usersGallery = new GalleryImage
			{
				Id = fileName,
				Caption = caption,
				DateUploaded = DateTime.Now
			};

			user.GalleryImages.Add(usersGallery);

			_applicationDbContext.SaveChanges();

			return usersGallery;
		}
	}
}
