namespace MyHub.Application.Extensions
{
    public static class StreamExtensions
    {
        public static MemoryStream ToMemoryStream(this string base64)
        {
			var image = base64[(base64.LastIndexOf(',') + 1)..];

			var bytes = Convert.FromBase64String(image);
            return new MemoryStream(bytes);
        }
    }
}
