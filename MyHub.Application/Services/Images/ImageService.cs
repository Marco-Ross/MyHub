using MyHub.Domain.Images.Interfaces;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace MyHub.Application.Services.Images
{
	public class ImageService : IImageService
	{

		public ImageService()
		{
		}

		public Stream GetQuantizedImageAsStream(Stream stream)
		{
			using Image<Rgba32> image = Image.Load<Rgba32>(stream);

			IQuantizer quantizer = new OctreeQuantizer();

			image.Mutate(x => x.Quantize(quantizer));

			var outputImageStream = new MemoryStream();

			image.Save(outputImageStream, new PngEncoder());

			outputImageStream.Position = 0;

			return outputImageStream;
		}

		public Stream CompressPng(Stream imageInput)
		{
			imageInput.Seek(0, SeekOrigin.Begin);

			using var image = Image.Load(imageInput);

			var memoryStream = new MemoryStream();

			var pngEncoder = new PngEncoder
			{
				CompressionLevel = PngCompressionLevel.BestCompression
			};

			image.Save(memoryStream, pngEncoder);

			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream;
		}
	}
}
