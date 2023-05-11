namespace MyHub.Application.Extensions
{
    public static class StreamExtensions
    {
        public static MemoryStream ToMemoryStream(this string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return new MemoryStream(bytes);
        }
    }
}
