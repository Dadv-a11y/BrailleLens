using BrailleLens.Core.Braille;
using BrailleLens.Core.Training;

namespace BrailleLens.Tests.Training;

public class WordBankTests
{
	[Fact]
	public void Words_only_use_supported_letters()
	{
		Assert.All(WordBank.Words, word => Assert.True(word.All(BrailleAlphabet.IsSupported), word));
		Assert.Equal(WordBank.Words.Count, WordBank.Words.Distinct().Count());
	}

	[Fact]
	public void Every_letter_appears_in_at_least_one_word()
	{
		Assert.All(BrailleAlphabet.Letters, letter => Assert.Contains(WordBank.Words, word => word.Contains(letter)));
	}

	[Fact]
	public void Pick_prefers_words_made_of_known_letters()
	{
		var random = new Random(7);

		for (var i = 0; i < 50; i++)
		{
			Assert.Contains(WordBank.Pick("ABCDEF".ToCharArray(), random), new[] { "BAC", "BEC", "FACE" });
		}
	}

	[Fact]
	public void Pick_falls_back_to_closest_words()
	{
		var word = WordBank.Pick(new[] { 'A' }, new Random(8));

		Assert.Contains(word, WordBank.Words);
	}

	[Fact]
	public void Pick_avoids_excluded_word()
	{
		var random = new Random(9);

		for (var i = 0; i < 50; i++)
		{
			Assert.NotEqual("BAC", WordBank.Pick("ABCDEF".ToCharArray(), random, "BAC"));
		}
	}
}
