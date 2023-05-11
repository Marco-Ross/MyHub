namespace MyHub.Domain.Enums.Enumerations
{
	public class StorageFolder : Enumeration
	{
		public static readonly StorageFolder ProfileImages = new("PI", "profile_images/");

		protected StorageFolder(string id, string name) : base(id, name)
		{
		}
	}
}
