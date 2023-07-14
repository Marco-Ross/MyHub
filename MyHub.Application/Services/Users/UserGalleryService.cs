using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Gallery;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Users
{
	public class UserGalleryService : IUserGalleryService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IAzureStorageService _azureStorageService;

		public UserGalleryService(ApplicationDbContext applicationDbContext, IAzureStorageService azureStorageService)
		{
			_applicationDbContext = applicationDbContext;
			_azureStorageService = azureStorageService;
		}

		public async Task<bool> RemoveUserImages(string userId)
		{
			var galleryImages = _applicationDbContext.GalleryImages.Where(x => x.UserCreatedId == userId).ToList();

			var tasks = new List<Task>();

			galleryImages.ForEach((galleryImage) =>
			{
				tasks.Add(RemoveFiles(galleryImage));
			});
			await Task.WhenAll(tasks);
			return true;
		}

		private async Task RemoveFiles(GalleryImage galleryImage)
		{
			var fileRemoved = await _azureStorageService.RemoveFile(GetGalleryImageStorageOptions(galleryImage.Id));

			if (fileRemoved)
				_applicationDbContext.GalleryImages.Remove(galleryImage);
		}

		public AzureStorageOptions GetGalleryImageStorageOptions(string fileName) => new()
		{
			StorageFolder = StorageFolder.GalleryImages,
			FileName = $"{fileName}.png",
			OverWrite = true
		};
	}
}
