using BrailleLens.Core.Braille;

namespace BrailleLens.Tests.Braille;

public class BrailleAlphabetTests
{
	[Theory]
	[InlineData('A', new[] { 1 })]
	[InlineData('B', new[] { 1, 2 })]
	[InlineData('J', new[] { 2, 4, 5 })]
	[InlineData('M', new[] { 1, 3, 4 })]
	[InlineData('O', new[] { 1, 3, 5 })]
	[InlineData('U', new[] { 1, 3, 6 })]
	[InlineData('W', new[] { 2, 4, 5, 6 })]
	[InlineData('Z', new[] { 1, 3, 5, 6 })]
	public void GetCell_returns_french_braille_dots(char letter, int[] dots)
	{
		Assert.Equal(dots, BrailleAlphabet.GetCell(letter).RaisedDots());
	}

	[Fact]
	public void Alphabet_has_26_distinct_cells()
	{
		Assert.Equal(26, BrailleAlphabet.Letters.Count);
		Assert.Equal(26, BrailleAlphabet.Letters.Select(BrailleAlphabet.GetCell).Distinct().Count());
	}

	[Fact]
	public void Letters_are_ordered_from_A_to_Z()
	{
		Assert.Equal('A', BrailleAlphabet.Letters[0]);
		Assert.Equal('Z', BrailleAlphabet.Letters[^1]);
		Assert.Equal(1, BrailleAlphabet.IndexOf('b'));
	}

	[Fact]
	public void Lookup_is_case_insensitive()
	{
		Assert.Equal(BrailleAlphabet.GetCell('Q'), BrailleAlphabet.GetCell('q'));
	}

	[Fact]
	public void TryGetLetter_inverts_GetCell()
	{
		foreach (var letter in BrailleAlphabet.Letters)
		{
			Assert.True(BrailleAlphabet.TryGetLetter(BrailleAlphabet.GetCell(letter), out var result));
			Assert.Equal(letter, result);
		}
	}

	[Theory]
	[InlineData('é')]
	[InlineData('1')]
	[InlineData('?')]
	public void Unsupported_characters_are_rejected(char symbol)
	{
		Assert.False(BrailleAlphabet.IsSupported(symbol));
		Assert.False(BrailleAlphabet.TryGetCell(symbol, out _));
		Assert.Throws<ArgumentException>(() => BrailleAlphabet.GetCell(symbol));
	}

	[Fact]
	public void Unknown_cell_has_no_letter()
	{
		Assert.False(BrailleAlphabet.TryGetLetter(BrailleCellModel.FromDots(6), out _));
	}
}
