namespace MyHub.Domain.Enums.Enumerations
{
	public class StorageFolder : Enumeration
	{
		public static readonly StorageFolder Default = new("D", "default/");
		public static readonly StorageFolder Test = new("T", "");
		public static readonly StorageFolder ProfileImages = new("PI", "profile_images/");
		public static readonly StorageFolder GalleryImages = new("GI", "gallery_images/");

		protected StorageFolder(string id, string name) : base(id, name)
		{
		}
	}
}
