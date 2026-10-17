using BrailleLens.Core.Scanning;
using Windows.Graphics.Imaging;

namespace BrailleLens.Services;

public static partial class ImageDecoder
{
	public static async partial Task<GrayImage> DecodeAsync(Stream stream, int maxSide)
	{
		var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
		var scale = Math.Min(1.0, maxSide / (double)Math.Max(decoder.OrientedPixelWidth, decoder.OrientedPixelHeight));
		var transform = new BitmapTransform
		{
			ScaledWidth = (uint)Math.Max(1, decoder.PixelWidth * scale),
			ScaledHeight = (uint)Math.Max(1, decoder.PixelHeight * scale),
			InterpolationMode = BitmapInterpolationMode.Fant,
		};

		var data = await decoder.GetPixelDataAsync(
			BitmapPixelFormat.Bgra8,
			BitmapAlphaMode.Ignore,
			transform,
			ExifOrientationMode.IgnoreExifOrientation,
			ColorManagementMode.DoNotColorManage);

		return GrayImage.FromRgba((int)transform.ScaledWidth, (int)transform.ScaledHeight, data.DetachPixelData(), isBgra: true);
	}
}
