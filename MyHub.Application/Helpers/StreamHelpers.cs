namespace MyHub.Application.Helpers
{
	public static class StreamHelpers
	{
		public static MemoryStream Base64ToMemoryStream(this string base64)
		{
			var bytes = Convert.FromBase64String(base64);
			return new MemoryStream(bytes);
		}
	}
}
