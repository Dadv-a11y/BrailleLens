using BrailleLens.Core.Braille;

namespace BrailleLens.Core.Learning;

public static class LevelPolicy
{
	public const int LettersPerLevel = 6;

	public static int MaxLevel => 1 + BrailleAlphabet.Letters.Count / LettersPerLevel;

	public static int GetLevel(int masteredCount)
	{
		var clamped = Math.Clamp(masteredCount, 0, BrailleAlphabet.Letters.Count);
		return 1 + clamped / LettersPerLevel;
	}

	public static int UnlockedLetterCount(int level) =>
		Math.Clamp(level * LettersPerLevel, LettersPerLevel, BrailleAlphabet.Letters.Count);
}
