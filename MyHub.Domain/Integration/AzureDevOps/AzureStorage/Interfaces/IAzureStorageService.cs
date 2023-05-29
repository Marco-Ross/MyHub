namespace MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces
{
	public interface IAzureStorageService
    {
        Task<bool> UploadFileToStorage(Stream fileStream, AzureStorageOptions storageOptions);
        Task<Stream?> GetFileFromStorage(AzureStorageOptions storageOptions);
        Task<bool> RemoveFile(AzureStorageOptions storageOptions);
    }
}
