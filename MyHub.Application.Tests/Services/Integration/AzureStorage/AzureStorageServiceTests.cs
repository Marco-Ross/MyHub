using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHub.Application.Services.Integration.AzureStorage;
using MyHub.Domain.ConfigurationOptions.Storage;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;

namespace MyHub.Application.Tests.Services.Integration.AzureStorage
{
	public class AzureStorageServiceTests : ApplicationTestBase
	{
		private readonly IAzureStorageService _sut;
		private readonly Mock<ILogger<AzureStorageService>> _logger = new();
		private readonly IOptions<StorageOptions> _storageOptions = Options.Create(GetStorageOptions());

		public AzureStorageServiceTests()
		{
			_sut = new AzureStorageService(_storageOptions, _logger.Object);
		}

		[Fact(Skip = "Integration test")]
		public async Task UploadFileToStorage_Upload_ReturnsTrue()
		{
			//Arrange
			var basePath = AppDomain.CurrentDomain.BaseDirectory;

			var path = Path.Combine(basePath, "Services", "Integration", "AzureStorage", "UploadTestFiles", "UploadTest.png");

			var inMemoryCopy = new MemoryStream();
			using (var fs = File.OpenRead(path))
			{
				fs.CopyTo(inMemoryCopy);
			}

			var options = new AzureStorageOptions { StorageFolder = StorageFolder.Test, FileName = "NewFile.png", OverWrite = true };

			//Act
			var uploaded = await _sut.UploadFileToStorage(inMemoryCopy, options);

			//Assert
			Assert.True(uploaded);
		}

		[Fact(Skip = "Integration test")]
		public async Task GetWorkItems_HasCache_ReturnsCache()
		{
			//Arrange
			var basePath = AppDomain.CurrentDomain.BaseDirectory;

			var path = Path.Combine(basePath, "Services", "Integration", "AzureStorage", "UploadTestFiles", "UploadTest.png");

			var inMemoryCopy = new MemoryStream();
			using (var fs = File.OpenRead(path))
			{
				fs.CopyTo(inMemoryCopy);
			}

			var options = new AzureStorageOptions { StorageFolder = StorageFolder.Test, FileName = "NewFile.png", OverWrite = true };

			//Act
			var uploaded = await _sut.GetFileFromStorage(options);

			//Assert
			Assert.NotNull(uploaded);
		}
	}
}
