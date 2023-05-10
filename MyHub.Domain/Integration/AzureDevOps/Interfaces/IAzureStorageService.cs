using MyHub.Domain.Enums.Enumerations;

namespace MyHub.Domain.Integration.AzureDevOps.Interfaces
{
	public interface IAzureStorageService
	{
		Task<bool> UploadFileToStorage(StorageFolder storageFolder, Stream fileStream, string fileName);
		Task<Stream?> GetFileFromStorage(StorageFolder storageFolder, string fileName);
	}
}
