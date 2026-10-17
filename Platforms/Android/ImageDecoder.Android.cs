using Android.Graphics;
using BrailleLens.Core.Scanning;

namespace BrailleLens.Services;

public static partial class ImageDecoder
{
	public static partial Task<GrayImage> DecodeAsync(Stream stream, int maxSide) => Task.Run(() =>
	{
		using var memory = new MemoryStream();
		stream.CopyTo(memory);
		var bytes = memory.ToArray();

		var bounds = new BitmapFactory.Options { InJustDecodeBounds = true };
		BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, bounds);
		var sampleSize = 1;
		while (Math.Max(bounds.OutWidth, bounds.OutHeight) / (sampleSize * 2) >= maxSide)
		{
			sampleSize *= 2;
		}

		using var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, new BitmapFactory.Options { InSampleSize = sampleSize })
			?? throw new InvalidOperationException("Image illisible.");

		var width = bitmap.Width;
		var height = bitmap.Height;
		var argb = new int[width * height];
		bitmap.GetPixels(argb, 0, width, 0, 0, width, height);

		var pixels = new byte[argb.Length];
		for (var i = 0; i < argb.Length; i++)
		{
			var color = argb[i];
			var r = (color >> 16) & 0xFF;
			var g = (color >> 8) & 0xFF;
			var b = color & 0xFF;
			pixels[i] = (byte)((r * 299 + g * 587 + b * 114) / 1000);
		}
		return new GrayImage(width, height, pixels);
	});
}
