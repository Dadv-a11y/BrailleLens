using BrailleLens.Core.Braille;

namespace BrailleLens.Tests.Braille;

public class BrailleDescriberTests
{
	[Fact]
	public void DescribeDots_lists_raised_dots_in_french()
	{
		Assert.Equal("aucun point", BrailleDescriber.DescribeDots(BrailleCellModel.Empty));
		Assert.Equal("point 1", BrailleDescriber.DescribeDots(BrailleCellModel.FromDots(1)));
		Assert.Equal("points 1 et 2", BrailleDescriber.DescribeDots(BrailleCellModel.FromDots(1, 2)));
		Assert.Equal("points 1, 4 et 5", BrailleDescriber.DescribeDots(BrailleCellModel.FromDots(5, 1, 4)));
	}

	[Fact]
	public void DescribeLetter_matches_learning_sentence()
	{
		Assert.Equal("Les points 1 et 2 forment le B.", BrailleDescriber.DescribeLetter('b'));
		Assert.Equal("Le point 1 forme le A.", BrailleDescriber.DescribeLetter('A'));
	}
}
