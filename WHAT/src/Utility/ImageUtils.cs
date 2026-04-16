using OnixRuntime.Api.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WHAT.Utility;

public static class ImageUtils {
	public static RawImageData ImageToRaw(Image<Bgra32> image) {
		using Image<Rgba32> rgba = image.CloneAs<Rgba32>();

		byte[] data = new byte[rgba.Width * rgba.Height * 4];
		rgba.CopyPixelDataTo(data);

		return new RawImageData(data, rgba.Width, rgba.Height);
	}
}