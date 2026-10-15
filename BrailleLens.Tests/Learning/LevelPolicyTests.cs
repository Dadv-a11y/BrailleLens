using BrailleLens.Core.Learning;

namespace BrailleLens.Tests.Learning;

public class LevelPolicyTests
{
	[Theory]
	[InlineData(0, 1)]
	[InlineData(5, 1)]
	[InlineData(6, 2)]
	[InlineData(8, 2)]
	[InlineData(10, 2)]
	[InlineData(12, 3)]
	[InlineData(26, 5)]
	[InlineData(40, 5)]
	[InlineData(-3, 1)]
	public void GetLevel_follows_six_letters_per_level(int mastered, int expected)
	{
		Assert.Equal(expected, LevelPolicy.GetLevel(mastered));
	}

	[Fact]
	public void MaxLevel_is_reached_with_all_letters()
	{
		Assert.Equal(LevelPolicy.MaxLevel, LevelPolicy.GetLevel(26));
	}

	[Theory]
	[InlineData(1, 6)]
	[InlineData(2, 12)]
	[InlineData(5, 26)]
	public void UnlockedLetterCount_grows_with_level(int level, int expected)
	{
		Assert.Equal(expected, LevelPolicy.UnlockedLetterCount(level));
	}
}
