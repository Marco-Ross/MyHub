using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Storage;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;

namespace MyHub.Application.Services.Integration.AzureStorage
{
	public class AzureStorageService : IAzureStorageService
	{
		private readonly StorageOptions _storageOptions;
		private readonly ILogger<AzureStorageService> _logger;

		public AzureStorageService(IOptions<StorageOptions> storageOptions, ILogger<AzureStorageService> logger)
		{
			_storageOptions = storageOptions.Value;
			_logger = logger;
		}

		private BlobClient GetBlobClient(StorageFolder storageFolder, string fileName)
		{
			var blobUri = new Uri(_storageOptions.BaseUrl + storageFolder.Name + fileName);

			var storageCredentials = new StorageSharedKeyCredential(_storageOptions.AccountName, _storageOptions.AccountKey);

			return new BlobClient(blobUri, storageCredentials);
		}

		public async Task<bool> UploadFileToStorage(StorageFolder storageFolder, Stream fileStream, string fileName)
		{
			var blobClient = GetBlobClient(storageFolder, fileName);

			fileStream.Position = 0;

			try
			{
				await blobClient.UploadAsync(fileStream);

				return await Task.FromResult(true);
			}
			catch (RequestFailedException exception)
			{
				_logger.LogError(exception, "Failed to upload file to storage");
				return false;
			}
		}

		public async Task<Stream?> GetFileFromStorage(StorageFolder storageFolder, string fileName)
		{
			var blobClient = GetBlobClient(storageFolder, fileName);

			try
			{
				var downloadResponse = await blobClient.DownloadContentAsync();

				return downloadResponse.Value.Content.ToStream();
			}
			catch (RequestFailedException exception)
			{
				_logger.LogError(exception, "Failed to upload file to storage");
				return null;
			}
		}
	}
}