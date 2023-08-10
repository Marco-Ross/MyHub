namespace MyHub.Application.Extensions
{
	public static class StringExtensions
	{
		public static string AsPng(this string fileName)
		{
			return $"{fileName}.png";
		}
	}
}
