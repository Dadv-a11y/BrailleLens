namespace BrailleLens.Core.Scanning;

public sealed class GrayImage
{
	public GrayImage(int width, int height, byte[] pixels)
	{
		if (width <= 0 || height <= 0 || pixels.Length != width * height)
		{
			throw new ArgumentException("Les dimensions ne correspondent pas aux pixels.", nameof(pixels));
		}

		Width = width;
		Height = height;
		Pixels = pixels;
	}

	public int Width { get; }

	public int Height { get; }

	public byte[] Pixels { get; }

	public byte this[int x, int y] => Pixels[y * Width + x];

	public GrayImage Rotate90()
	{
		var rotated = new byte[Pixels.Length];
		for (var y = 0; y < Height; y++)
		{
			for (var x = 0; x < Width; x++)
			{
				rotated[x * Height + (Height - 1 - y)] = Pixels[y * Width + x];
			}
		}
		return new GrayImage(Height, Width, rotated);
	}

	public GrayImage Downscale(int maxSide)
	{
		var factor = (int)Math.Ceiling(Math.Max(Width, Height) / (double)maxSide);
		if (factor <= 1)
		{
			return this;
		}

		var width = Width / factor;
		var height = Height / factor;
		var pixels = new byte[width * height];
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var sum = 0;
				for (var dy = 0; dy < factor; dy++)
				{
					for (var dx = 0; dx < factor; dx++)
					{
						sum += this[x * factor + dx, y * factor + dy];
					}
				}
				pixels[y * width + x] = (byte)(sum / (factor * factor));
			}
		}
		return new GrayImage(width, height, pixels);
	}

	public static GrayImage FromRgba(int width, int height, ReadOnlySpan<byte> rgba, bool isBgra = false)
	{
		var pixels = new byte[width * height];
		for (var i = 0; i < pixels.Length; i++)
		{
			var r = rgba[i * 4 + (isBgra ? 2 : 0)];
			var g = rgba[i * 4 + 1];
			var b = rgba[i * 4 + (isBgra ? 0 : 2)];
			pixels[i] = (byte)((r * 299 + g * 587 + b * 114) / 1000);
		}
		return new GrayImage(width, height, pixels);
	}
}
