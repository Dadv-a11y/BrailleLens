using BrailleLens.Core.Braille;

namespace BrailleLens.Core.Scanning;

public static class CellSegmenter
{
	private const double SameCellColumnFactor = 1.2;
	private const double ClusterTolerance = 0.45;
	private const double SpaceFactor = 1.6;

	public static IReadOnlyList<BrailleCellModel> Segment(IReadOnlyList<Dot> dots)
	{
		if (dots.Count == 0)
		{
			return Array.Empty<BrailleCellModel>();
		}

		var pitch = EstimatePitch(dots);
		if (pitch <= 0)
		{
			return Array.Empty<BrailleCellModel>();
		}

		var lineDots = KeepMainLine(dots, pitch);
		var top = lineDots.Min(dot => dot.Y);
		var columns = Cluster(lineDots.Select(dot => dot.X), pitch * ClusterTolerance);

		var cellStarts = new List<double>();
		var rightColumns = new Dictionary<int, double>();
		for (var i = 0; i < columns.Count; i++)
		{
			cellStarts.Add(columns[i]);
			if (i + 1 < columns.Count && columns[i + 1] - columns[i] < pitch * SameCellColumnFactor)
			{
				rightColumns[cellStarts.Count - 1] = columns[i + 1];
				i++;
			}
		}

		var cells = new BrailleDots[cellStarts.Count];
		foreach (var dot in lineDots)
		{
			var row = (int)Math.Round((dot.Y - top) / pitch);
			if (row is < 0 or > 2)
			{
				continue;
			}

			var (cellIndex, isRight) = LocateColumn(dot.X, cellStarts, rightColumns);
			var dotNumber = row + 1 + (isRight ? 3 : 0);
			cells[cellIndex] |= (BrailleDots)(1 << (dotNumber - 1));
		}

		return WithSpaces(cells, cellStarts, pitch);
	}

	private static double EstimatePitch(IReadOnlyList<Dot> dots)
	{
		var radius = Math.Sqrt(dots.Select(dot => dot.Area).Order().ElementAt(dots.Count / 2) / Math.PI);
		var fallback = radius * 3.3;
		var tolerance = radius * 1.2;

		var gaps = Gaps(Cluster(dots.Select(dot => dot.X), tolerance))
			.Concat(Gaps(Cluster(dots.Select(dot => dot.Y), tolerance)))
			.Where(gap => gap >= radius * 2.5)
			.Order()
			.ToArray();
		if (gaps.Length == 0)
		{
			return fallback;
		}

		var smallest = gaps[0];
		return gaps.Where(gap => gap <= smallest * 1.2).Average();
	}

	private static IEnumerable<double> Gaps(List<double> centers) =>
		centers.Zip(centers.Skip(1), (a, b) => b - a);

	private static List<Dot> KeepMainLine(IReadOnlyList<Dot> dots, double pitch)
	{
		var rows = Cluster(dots.Select(dot => dot.Y), pitch * ClusterTolerance);
		var best = rows
			.Select(top => dots.Where(dot => dot.Y >= top - pitch * ClusterTolerance && dot.Y <= top + pitch * (2 + ClusterTolerance)).ToList())
			.OrderByDescending(group => group.Count)
			.First();
		return best;
	}

	private static List<double> Cluster(IEnumerable<double> values, double tolerance)
	{
		var sorted = values.Order().ToArray();
		var centers = new List<double>();
		var group = new List<double>();
		foreach (var value in sorted)
		{
			if (group.Count > 0 && value - group.Average() > tolerance)
			{
				centers.Add(group.Average());
				group.Clear();
			}
			group.Add(value);
		}
		if (group.Count > 0)
		{
			centers.Add(group.Average());
		}
		return centers;
	}

	private static (int CellIndex, bool IsRight) LocateColumn(double x, List<double> cellStarts, Dictionary<int, double> rightColumns)
	{
		var bestIndex = 0;
		var bestIsRight = false;
		var bestDistance = double.MaxValue;
		for (var i = 0; i < cellStarts.Count; i++)
		{
			var left = Math.Abs(x - cellStarts[i]);
			if (left < bestDistance)
			{
				(bestIndex, bestIsRight, bestDistance) = (i, false, left);
			}
			if (rightColumns.TryGetValue(i, out var rightX) && Math.Abs(x - rightX) < bestDistance)
			{
				(bestIndex, bestIsRight, bestDistance) = (i, true, Math.Abs(x - rightX));
			}
		}
		return (bestIndex, bestIsRight);
	}

	private static IReadOnlyList<BrailleCellModel> WithSpaces(BrailleDots[] cells, List<double> cellStarts, double pitch)
	{
		var gaps = cellStarts.Zip(cellStarts.Skip(1), (a, b) => b - a).Order().ToArray();
		var cellPitch = gaps.Length > 0 ? Math.Min(gaps[gaps.Length / 2], pitch * 3) : pitch * 2.4;
		cellPitch = Math.Max(cellPitch, pitch * 2);

		var result = new List<BrailleCellModel>();
		for (var i = 0; i < cells.Length; i++)
		{
			if (i > 0)
			{
				var gap = cellStarts[i] - cellStarts[i - 1];
				var missing = (int)Math.Round(gap / cellPitch) - 1;
				if (gap > cellPitch * SpaceFactor)
				{
					for (var s = 0; s < Math.Max(1, missing); s++)
					{
						result.Add(BrailleCellModel.Empty);
					}
				}
			}
			result.Add(new BrailleCellModel(cells[i]));
		}
		return result;
	}
}
