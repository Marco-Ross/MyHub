namespace MyHub.Domain.ConfigurationOptions.Storage
{
	public class StorageOptions
	{
		public string AccountName { get; set; } = string.Empty;
		public string ImageContainer { get; set; } = string.Empty;
		public string AccountKey { get; set; } = string.Empty;
		public string BaseUrl { get; set; } = string.Empty;
	}
}
