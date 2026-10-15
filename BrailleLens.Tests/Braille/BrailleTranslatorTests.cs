using BrailleLens.Core.Braille;

namespace BrailleLens.Tests.Braille;

public class BrailleTranslatorTests
{
	[Fact]
	public void Bonjour_gives_7_letters_and_7_cells()
	{
		var translation = BrailleTranslator.ToBraille("Bonjour");

		Assert.Equal(7, translation.LetterCount);
		Assert.Equal(7, translation.CellCount);
		Assert.False(translation.HasUnsupported);
		Assert.Equal("BONJOUR", new string(translation.Tokens.Select(token => token.Source).ToArray()));
		Assert.Equal("⠃⠕⠝⠚⠕⠥⠗", translation.ToUnicode());
	}

	[Fact]
	public void Spaces_become_empty_cells()
	{
		var translation = BrailleTranslator.ToBraille("a b");

		Assert.Equal(2, translation.LetterCount);
		Assert.Equal(3, translation.CellCount);
		Assert.Equal(BrailleTokenKind.Space, translation.Tokens[1].Kind);
		Assert.True(translation.Tokens[1].Cell.IsEmpty);
		Assert.Equal("⠁ ⠃", translation.ToUnicode());
	}

	[Fact]
	public void Unsupported_characters_are_reported_and_kept()
	{
		var translation = BrailleTranslator.ToBraille("été 2!");

		Assert.True(translation.HasUnsupported);
		Assert.Equal(new[] { 'é', '2', '!' }, translation.UnsupportedCharacters);
		Assert.Equal(1, translation.LetterCount);
		Assert.Equal("é⠞é 2!", translation.ToUnicode());
	}

	[Fact]
	public void Empty_or_null_text_gives_no_tokens()
	{
		Assert.Empty(BrailleTranslator.ToBraille(null).Tokens);
		Assert.Empty(BrailleTranslator.ToBraille(string.Empty).Tokens);
	}

	[Fact]
	public void ToText_reads_cells_back()
	{
		var cells = "AMI".Select(BrailleAlphabet.GetCell).Append(BrailleCellModel.Empty);

		Assert.Equal("AMI ", BrailleTranslator.ToText(cells));
	}

	[Fact]
	public void ToText_marks_unknown_cells()
	{
		Assert.Equal("?", BrailleTranslator.ToText(new[] { BrailleCellModel.FromDots(6) }));
	}

	[Fact]
	public void UnicodeToText_round_trips_with_ToUnicode()
	{
		var unicode = BrailleTranslator.ToBraille("salut toi").ToUnicode();

		Assert.Equal("SALUT TOI", BrailleTranslator.UnicodeToText(unicode));
	}
}
