using MyHub.Domain.Images.Interfaces;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace MyHub.Application.Services.Images
{
	public class ImageQuantizationService : IImageQuantizationService
	{

		public ImageQuantizationService()
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
	}
}
