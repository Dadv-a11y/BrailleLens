namespace BrailleLens.Core.Braille;

public static class BrailleAlphabet
{
	private static readonly Dictionary<char, BrailleCellModel> LetterToCell = new()
	{
		['A'] = BrailleCellModel.FromDots(1),
		['B'] = BrailleCellModel.FromDots(1, 2),
		['C'] = BrailleCellModel.FromDots(1, 4),
		['D'] = BrailleCellModel.FromDots(1, 4, 5),
		['E'] = BrailleCellModel.FromDots(1, 5),
		['F'] = BrailleCellModel.FromDots(1, 2, 4),
		['G'] = BrailleCellModel.FromDots(1, 2, 4, 5),
		['H'] = BrailleCellModel.FromDots(1, 2, 5),
		['I'] = BrailleCellModel.FromDots(2, 4),
		['J'] = BrailleCellModel.FromDots(2, 4, 5),
		['K'] = BrailleCellModel.FromDots(1, 3),
		['L'] = BrailleCellModel.FromDots(1, 2, 3),
		['M'] = BrailleCellModel.FromDots(1, 3, 4),
		['N'] = BrailleCellModel.FromDots(1, 3, 4, 5),
		['O'] = BrailleCellModel.FromDots(1, 3, 5),
		['P'] = BrailleCellModel.FromDots(1, 2, 3, 4),
		['Q'] = BrailleCellModel.FromDots(1, 2, 3, 4, 5),
		['R'] = BrailleCellModel.FromDots(1, 2, 3, 5),
		['S'] = BrailleCellModel.FromDots(2, 3, 4),
		['T'] = BrailleCellModel.FromDots(2, 3, 4, 5),
		['U'] = BrailleCellModel.FromDots(1, 3, 6),
		['V'] = BrailleCellModel.FromDots(1, 2, 3, 6),
		['W'] = BrailleCellModel.FromDots(2, 4, 5, 6),
		['X'] = BrailleCellModel.FromDots(1, 3, 4, 6),
		['Y'] = BrailleCellModel.FromDots(1, 3, 4, 5, 6),
		['Z'] = BrailleCellModel.FromDots(1, 3, 5, 6),
	};

	private static readonly Dictionary<BrailleCellModel, char> CellToLetter =
		LetterToCell.ToDictionary(pair => pair.Value, pair => pair.Key);

	private static readonly char[] OrderedLetters = LetterToCell.Keys.Order().ToArray();

	public static IReadOnlyList<char> Letters => OrderedLetters;

	public static bool IsSupported(char letter) => LetterToCell.ContainsKey(char.ToUpperInvariant(letter));

	public static BrailleCellModel GetCell(char letter) =>
		TryGetCell(letter, out var cell)
			? cell
			: throw new ArgumentException($"La lettre « {letter} » n'est pas prise en charge.", nameof(letter));

	public static bool TryGetCell(char letter, out BrailleCellModel cell) =>
		LetterToCell.TryGetValue(char.ToUpperInvariant(letter), out cell);

	public static bool TryGetLetter(BrailleCellModel cell, out char letter) =>
		CellToLetter.TryGetValue(cell, out letter);

	public static int IndexOf(char letter) => Array.IndexOf(OrderedLetters, char.ToUpperInvariant(letter));
}
