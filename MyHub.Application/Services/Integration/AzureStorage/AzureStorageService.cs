using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHub.Domain.ConfigurationOptions.Storage;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;

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

		private BlobClient GetBlobClient(AzureStorageOptions storageOptions)
		{
			var blobUri = new Uri(_storageOptions.BaseUrl + _storageOptions.BaseFolder + storageOptions.StorageFolder.Name + storageOptions.FileName);

			var storageCredentials = new StorageSharedKeyCredential(_storageOptions.AccountName, _storageOptions.AccountKey);

			return new BlobClient(blobUri, storageCredentials);
		}

		public async Task<bool> UploadFileToStorage(Stream fileStream, AzureStorageOptions storageOptions)
		{
			var blobClient = GetBlobClient(storageOptions);

			fileStream.Position = 0;

			try
			{
				await blobClient.UploadAsync(fileStream, overwrite: storageOptions.OverWrite);

				return await Task.FromResult(true);
			}
			catch (RequestFailedException exception)
			{
				_logger.LogError(exception, "Failed to upload file to storage");
				return false;
			}
		}

		public async Task<Stream?> GetFileFromStorage(AzureStorageOptions storageOptions)
		{
			var blobClient = GetBlobClient(storageOptions);

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

		public async Task<bool> RemoveFile(AzureStorageOptions storageOptions)
		{
			var blobClient = GetBlobClient(storageOptions);

			try
			{
				var downloadResponse = await blobClient.DeleteIfExistsAsync();

				return downloadResponse.Value;
			}
			catch (RequestFailedException exception)
			{
				_logger.LogError(exception, "Failed to upload file to storage");
				return false;
			}
		}
	}
}