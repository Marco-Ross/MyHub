namespace MyHub.Domain.Images.Interfaces
{
	public interface IImageService
	{
		Stream GetQuantizedImageAsStream(Stream stream);
		Stream CompressPng(Stream imageInput);
	}
}
