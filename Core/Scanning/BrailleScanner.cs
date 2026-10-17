using BrailleLens.Core.Braille;

namespace BrailleLens.Core.Scanning;

public sealed record ScanResult(IReadOnlyList<BrailleCellModel> Cells, int DotCount)
{
	public static ScanResult Empty { get; } = new(Array.Empty<BrailleCellModel>(), 0);

	public bool HasCells => Cells.Any(cell => !cell.IsEmpty);

	public string Text => BrailleTranslator.ToText(Cells).Trim();

	public int UnknownCount => Cells.Count(cell => !cell.IsEmpty && !BrailleAlphabet.TryGetLetter(cell, out _));
}

public static class BrailleScanner
{
	public const int WorkingSize = 1000;

	public static ScanResult Recognize(GrayImage image)
	{
		var working = image.Downscale(WorkingSize);
		var best = ScanResult.Empty;

		for (var turn = 0; turn < 4; turn++)
		{
			foreach (var darkDots in new[] { true, false })
			{
				var dots = DotDetector.Detect(working, darkDots);
				var candidate = new ScanResult(CellSegmenter.Segment(dots), dots.Count);
				if (IsBetter(candidate, best))
				{
					best = candidate;
				}
			}
			working = working.Rotate90();
		}
		return best;
	}

	private static bool IsBetter(ScanResult candidate, ScanResult current)
	{
		if (!candidate.HasCells)
		{
			return false;
		}
		if (!current.HasCells)
		{
			return true;
		}

		var candidateKnown = candidate.Cells.Count(cell => !cell.IsEmpty) - candidate.UnknownCount;
		var currentKnown = current.Cells.Count(cell => !cell.IsEmpty) - current.UnknownCount;
		if (candidate.UnknownCount != current.UnknownCount)
		{
			return candidate.UnknownCount < current.UnknownCount && candidateKnown >= currentKnown;
		}
		return candidateKnown > currentKnown;
	}
}
