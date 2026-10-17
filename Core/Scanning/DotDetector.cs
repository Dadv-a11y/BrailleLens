namespace BrailleLens.Core.Scanning;

public readonly record struct Dot(double X, double Y, int Area);

public static class DotDetector
{
	private const int ContrastOffset = 18;
	private const double MinFillRatio = 0.45;
	private const double MaxAspectRatio = 2.0;

	public static IReadOnlyList<Dot> Detect(GrayImage image, bool darkDots = true)
	{
		var mask = Binarize(image, darkDots);
		var blobs = FindBlobs(mask, image.Width, image.Height);
		return FilterBySize(blobs);
	}

	private static bool[] Binarize(GrayImage image, bool darkDots)
	{
		var width = image.Width;
		var height = image.Height;
		var integral = new long[(width + 1) * (height + 1)];
		for (var y = 0; y < height; y++)
		{
			long rowSum = 0;
			for (var x = 0; x < width; x++)
			{
				rowSum += image[x, y];
				integral[(y + 1) * (width + 1) + x + 1] = integral[y * (width + 1) + x + 1] + rowSum;
			}
		}

		var radius = Math.Max(4, Math.Min(width, height) / 12);
		var mask = new bool[width * height];
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var mean = LocalMean(integral, width, height, x, y, radius);
				var value = LocalMean(integral, width, height, x, y, 1);
				mask[y * width + x] = darkDots ? value < mean - ContrastOffset : value > mean + ContrastOffset;
			}
		}
		return mask;
	}

	private static double LocalMean(long[] integral, int width, int height, int x, int y, int radius)
	{
		var x0 = Math.Max(0, x - radius);
		var x1 = Math.Min(width, x + radius + 1);
		var y0 = Math.Max(0, y - radius);
		var y1 = Math.Min(height, y + radius + 1);
		var sum = integral[y1 * (width + 1) + x1] - integral[y0 * (width + 1) + x1]
			- integral[y1 * (width + 1) + x0] + integral[y0 * (width + 1) + x0];
		return sum / (double)((x1 - x0) * (y1 - y0));
	}

	private static List<(Dot Dot, int Width, int Height)> FindBlobs(bool[] mask, int width, int height)
	{
		var blobs = new List<(Dot, int, int)>();
		var visited = new bool[mask.Length];
		var stack = new Stack<int>();

		for (var start = 0; start < mask.Length; start++)
		{
			if (!mask[start] || visited[start])
			{
				continue;
			}

			long sumX = 0, sumY = 0;
			int area = 0, minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
			stack.Push(start);
			visited[start] = true;

			while (stack.Count > 0)
			{
				var index = stack.Pop();
				var x = index % width;
				var y = index / width;
				area++;
				sumX += x;
				sumY += y;
				minX = Math.Min(minX, x);
				maxX = Math.Max(maxX, x);
				minY = Math.Min(minY, y);
				maxY = Math.Max(maxY, y);

				if (x > 0) Visit(index - 1);
				if (x < width - 1) Visit(index + 1);
				if (y > 0) Visit(index - width);
				if (y < height - 1) Visit(index + width);
			}

			var blobWidth = maxX - minX + 1;
			var blobHeight = maxY - minY + 1;
			var aspect = Math.Max(blobWidth, blobHeight) / (double)Math.Min(blobWidth, blobHeight);
			var fill = area / (double)(blobWidth * blobHeight);
			if (area >= 4 && aspect <= MaxAspectRatio && fill >= MinFillRatio)
			{
				blobs.Add((new Dot(sumX / (double)area, sumY / (double)area, area), blobWidth, blobHeight));
			}
		}
		return blobs;

		void Visit(int index)
		{
			if (mask[index] && !visited[index])
			{
				visited[index] = true;
				stack.Push(index);
			}
		}
	}

	private static IReadOnlyList<Dot> FilterBySize(List<(Dot Dot, int Width, int Height)> blobs)
	{
		if (blobs.Count == 0)
		{
			return Array.Empty<Dot>();
		}

		var areas = blobs.Select(blob => blob.Dot.Area).Order().ToArray();
		var bestCluster = areas
			.Select(area => areas.Count(other => other >= area * 0.5 && other <= area * 2.0))
			.Max();
		var reference = areas.First(area => areas.Count(other => other >= area * 0.5 && other <= area * 2.0) == bestCluster);

		return blobs
			.Where(blob => blob.Dot.Area >= reference * 0.5 && blob.Dot.Area <= reference * 2.0)
			.Select(blob => blob.Dot)
			.ToArray();
	}
}
