using BrailleLens.Core.Braille;

namespace BrailleLens.Tests.Braille;

public class BrailleCellModelTests
{
	[Fact]
	public void Toggle_raises_then_lowers_a_dot()
	{
		var cell = BrailleCellModel.Empty.Toggle(3);
		Assert.True(cell.IsRaised(3));
		Assert.False(cell.Toggle(3).IsRaised(3));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(7)]
	public void Dots_outside_1_to_6_are_rejected(int dot)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => BrailleCellModel.Empty.Toggle(dot));
	}

	[Fact]
	public void Unicode_matches_braille_patterns_block()
	{
		Assert.Equal('⠀', BrailleCellModel.Empty.ToUnicode());
		Assert.Equal('⠃', BrailleCellModel.FromDots(1, 2).ToUnicode());
		Assert.Equal('⠿', new BrailleCellModel(BrailleDots.All).ToUnicode());
	}

	[Fact]
	public void TryFromUnicode_round_trips_all_cells()
	{
		for (var value = 0; value <= (int)BrailleDots.All; value++)
		{
			var cell = new BrailleCellModel((BrailleDots)value);
			Assert.True(BrailleCellModel.TryFromUnicode(cell.ToUnicode(), out var parsed));
			Assert.Equal(cell, parsed);
		}
		Assert.False(BrailleCellModel.TryFromUnicode('a', out _));
	}

	[Fact]
	public void DistanceTo_counts_differing_dots()
	{
		Assert.Equal(1, BrailleCellModel.FromDots(1).DistanceTo(BrailleCellModel.FromDots(1, 2)));
		Assert.Equal(3, BrailleCellModel.FromDots(1, 2).DistanceTo(BrailleCellModel.FromDots(2, 4, 5)));
	}

	[Theory]
	[InlineData("12", new[] { 1, 2 })]
	[InlineData("1, 4, 5", new[] { 1, 4, 5 })]
	[InlineData("b", new[] { 1, 2 })]
	[InlineData("", new int[0])]
	public void Converter_parses_dot_numbers_or_letter(string text, int[] dots)
	{
		var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(BrailleCellModel));

		Assert.Equal(dots, ((BrailleCellModel)converter.ConvertFromInvariantString(text)!).RaisedDots());
	}

	[Fact]
	public void RaisedCount_counts_raised_dots()
	{
		Assert.Equal(4, BrailleCellModel.FromDots(1, 3, 4, 5).RaisedCount);
	}
}
