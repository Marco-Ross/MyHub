using MyHub.Domain.Enums.Enumerations;

namespace MyHub.Domain.Integration.AzureDevOps.AzureStorage
{
	public class AzureStorageOptions
	{
		public StorageFolder StorageFolder { get; set; } = StorageFolder.Default;
		public string FileName { get; set; } = string.Empty;
		public bool OverWrite { get; set; }
	}
}
