using BrailleLens.Core.Braille;
using BrailleLens.Core.Scanning;

namespace BrailleLens.Tests.Scanning;

internal static class SyntheticBraille
{
	public static GrayImage Render(string text, double dotPitch = 20, int noise = 0, bool inverted = false, int seed = 1)
	{
		var cellPitch = dotPitch * 2.4;
		var radius = dotPitch * 0.3;
		var margin = (int)(dotPitch * 3);
		var width = (int)(margin * 2 + cellPitch * text.Length);
		var height = (int)(margin * 2 + dotPitch * 2);
		var background = inverted ? (byte)40 : (byte)225;
		var ink = inverted ? (byte)220 : (byte)35;
		var pixels = Enumerable.Repeat(background, width * height).ToArray();

		for (var index = 0; index < text.Length; index++)
		{
			if (text[index] == ' ')
			{
				continue;
			}

			var cell = BrailleAlphabet.GetCell(text[index]);
			foreach (var dot in cell.RaisedDots())
			{
				var column = dot > 3 ? 1 : 0;
				var row = (dot - 1) % 3;
				var cx = margin + index * cellPitch + column * dotPitch;
				var cy = margin + row * dotPitch;
				for (var y = (int)(cy - radius); y <= (int)(cy + radius); y++)
				{
					for (var x = (int)(cx - radius); x <= (int)(cx + radius); x++)
					{
						if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= radius * radius)
						{
							pixels[y * width + x] = ink;
						}
					}
				}
			}
		}

		if (noise > 0)
		{
			var random = new Random(seed);
			for (var i = 0; i < pixels.Length; i++)
			{
				pixels[i] = (byte)Math.Clamp(pixels[i] + random.Next(-noise, noise + 1), 0, 255);
			}
		}

		return new GrayImage(width, height, pixels);
	}
}
