using System.Text;

namespace BrailleLens.Core.Braille;

public enum BrailleTokenKind
{
	Letter,
	Space,
	Unsupported
}

public readonly record struct BrailleToken(char Source, BrailleTokenKind Kind, BrailleCellModel Cell)
{
	public bool HasCell => Kind != BrailleTokenKind.Unsupported;
}

public sealed class BrailleTranslation
{
	public BrailleTranslation(IReadOnlyList<BrailleToken> tokens)
	{
		Tokens = tokens;
	}

	public IReadOnlyList<BrailleToken> Tokens { get; }

	public int LetterCount => Tokens.Count(token => token.Kind == BrailleTokenKind.Letter);

	public int CellCount => Tokens.Count(token => token.HasCell);

	public bool HasUnsupported => Tokens.Any(token => token.Kind == BrailleTokenKind.Unsupported);

	public IReadOnlyList<char> UnsupportedCharacters =>
		Tokens.Where(token => token.Kind == BrailleTokenKind.Unsupported).Select(token => token.Source).Distinct().ToArray();

	public string ToUnicode()
	{
		var builder = new StringBuilder(Tokens.Count);
		foreach (var token in Tokens)
		{
			builder.Append(token.Kind switch
			{
				BrailleTokenKind.Letter => token.Cell.ToUnicode(),
				BrailleTokenKind.Space => ' ',
				_ => token.Source
			});
		}
		return builder.ToString();
	}
}

public static class BrailleTranslator
{
	public const char UnknownLetter = '?';

	public static BrailleTranslation ToBraille(string? text)
	{
		var tokens = new List<BrailleToken>(text?.Length ?? 0);
		foreach (var symbol in text ?? string.Empty)
		{
			if (char.IsWhiteSpace(symbol))
			{
				tokens.Add(new BrailleToken(symbol, BrailleTokenKind.Space, BrailleCellModel.Empty));
			}
			else if (BrailleAlphabet.TryGetCell(symbol, out var cell))
			{
				tokens.Add(new BrailleToken(char.ToUpperInvariant(symbol), BrailleTokenKind.Letter, cell));
			}
			else
			{
				tokens.Add(new BrailleToken(symbol, BrailleTokenKind.Unsupported, BrailleCellModel.Empty));
			}
		}
		return new BrailleTranslation(tokens);
	}

	public static BrailleTranslation FromCells(IEnumerable<BrailleCellModel> cells) =>
		new(cells.Select(cell => cell.IsEmpty
			? new BrailleToken(' ', BrailleTokenKind.Space, cell)
			: new BrailleToken(ToLetter(cell), BrailleTokenKind.Letter, cell)).ToArray());

	public static char ToLetter(BrailleCellModel cell)
	{
		if (cell.IsEmpty)
		{
			return ' ';
		}
		return BrailleAlphabet.TryGetLetter(cell, out var letter) ? letter : UnknownLetter;
	}

	public static string ToText(IEnumerable<BrailleCellModel> cells)
	{
		var builder = new StringBuilder();
		foreach (var cell in cells)
		{
			builder.Append(ToLetter(cell));
		}
		return builder.ToString();
	}

	public static string UnicodeToText(string? unicode)
	{
		var builder = new StringBuilder(unicode?.Length ?? 0);
		foreach (var symbol in unicode ?? string.Empty)
		{
			builder.Append(BrailleCellModel.TryFromUnicode(symbol, out var cell) ? ToLetter(cell) : symbol);
		}
		return builder.ToString();
	}
}
