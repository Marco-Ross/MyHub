namespace MyHub.Domain.Images.Interfaces
{
	public interface IImageQuantizationService
	{
		Stream GetQuantizedImageAsStream(Stream stream);
	}
}
