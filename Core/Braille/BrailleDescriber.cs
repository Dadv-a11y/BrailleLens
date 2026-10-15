namespace BrailleLens.Core.Braille;

public static class BrailleDescriber
{
	public static string DescribeDots(BrailleCellModel cell)
	{
		var dots = cell.RaisedDots();
		return dots.Count switch
		{
			0 => "aucun point",
			1 => $"point {dots[0]}",
			_ => $"points {string.Join(", ", dots.Take(dots.Count - 1))} et {dots[^1]}"
		};
	}

	public static string DescribeLetter(char letter)
	{
		var cell = BrailleAlphabet.GetCell(letter);
		var upper = char.ToUpperInvariant(letter);
		return cell.RaisedCount > 1
			? $"Les {DescribeDots(cell)} forment le {upper}."
			: $"Le {DescribeDots(cell)} forme le {upper}.";
	}
}
