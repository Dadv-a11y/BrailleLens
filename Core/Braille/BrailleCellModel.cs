using System.ComponentModel;
using System.Numerics;

namespace BrailleLens.Core.Braille;

[TypeConverter(typeof(BrailleCellModelConverter))]
public readonly record struct BrailleCellModel(BrailleDots Dots)
{
	private const char UnicodeBase = '⠀';

	public static BrailleCellModel Empty => new(BrailleDots.None);

	public bool IsEmpty => Dots == BrailleDots.None;

	public int RaisedCount => BitOperations.PopCount((uint)Dots);

	public static BrailleCellModel FromDots(params int[] dots)
	{
		var value = BrailleDots.None;
		foreach (var dot in dots)
		{
			value |= ToFlag(dot);
		}
		return new BrailleCellModel(value);
	}

	public bool IsRaised(int dot) => (Dots & ToFlag(dot)) != 0;

	public BrailleCellModel Toggle(int dot) => new(Dots ^ ToFlag(dot));

	public IReadOnlyList<int> RaisedDots()
	{
		var result = new List<int>(6);
		for (var dot = 1; dot <= 6; dot++)
		{
			if (IsRaised(dot))
			{
				result.Add(dot);
			}
		}
		return result;
	}

	public int DistanceTo(BrailleCellModel other) => BitOperations.PopCount((uint)(Dots ^ other.Dots));

	public char ToUnicode() => (char)(UnicodeBase + (int)Dots);

	public static bool TryFromUnicode(char symbol, out BrailleCellModel cell)
	{
		var offset = symbol - UnicodeBase;
		if (offset is >= 0 and <= (int)BrailleDots.All)
		{
			cell = new BrailleCellModel((BrailleDots)offset);
			return true;
		}
		cell = Empty;
		return false;
	}

	private static BrailleDots ToFlag(int dot)
	{
		if (dot is < 1 or > 6)
		{
			throw new ArgumentOutOfRangeException(nameof(dot), dot, "Un point braille est numéroté de 1 à 6.");
		}
		return (BrailleDots)(1 << (dot - 1));
	}
}
