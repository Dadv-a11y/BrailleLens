using BrailleLens.Core.Braille;
using BrailleLens.Core.Learning;
using BrailleLens.Core.Training;

namespace BrailleLens.Tests.Training;

public class QuestionGeneratorTests
{
	[Theory]
	[InlineData(ExerciseType.RecognizeLetter)]
	[InlineData(ExerciseType.FindBraille)]
	public void Choice_questions_have_four_distinct_choices_with_answer(ExerciseType type)
	{
		var generator = new QuestionGenerator(new Random(1));
		var progress = new Progress();

		for (var i = 0; i < 200; i++)
		{
			var question = generator.Create(type, progress);

			Assert.Equal(QuestionGenerator.ChoiceCount, question.Choices.Distinct().Count());
			Assert.Contains(question.Letter, question.Choices);
		}
	}

	[Fact]
	public void Distractors_are_visually_close_to_the_answer()
	{
		var generator = new QuestionGenerator(new Random(2));
		var progress = new Progress();

		for (var i = 0; i < 200; i++)
		{
			var question = generator.Create(ExerciseType.RecognizeLetter, progress);
			var target = BrailleAlphabet.GetCell(question.Letter);
			var maxAllowed = BrailleAlphabet.Letters
				.Where(letter => letter != question.Letter)
				.Select(letter => BrailleAlphabet.GetCell(letter).DistanceTo(target))
				.Order()
				.ElementAt(5);

			Assert.All(question.Choices, choice =>
				Assert.True(BrailleAlphabet.GetCell(choice).DistanceTo(target) <= maxAllowed));
		}
	}

	[Fact]
	public void Letters_come_from_unlocked_letters()
	{
		var generator = new QuestionGenerator(new Random(3));
		var progress = new Progress();

		for (var i = 0; i < 200; i++)
		{
			var question = generator.Create(ExerciseType.WriteBraille, progress);

			Assert.Contains(question.Letter, progress.UnlockedLetters);
			Assert.Empty(question.Choices);
		}
	}

	[Fact]
	public void Previous_letter_is_not_repeated()
	{
		var generator = new QuestionGenerator(new Random(4));
		var progress = new Progress();
		var previous = "A";

		for (var i = 0; i < 200; i++)
		{
			var question = generator.Create(ExerciseType.FindBraille, progress, previous);

			Assert.NotEqual(previous, question.Target);
			previous = question.Target;
		}
	}

	[Fact]
	public void Unmastered_letters_are_asked_more_often()
	{
		var generator = new QuestionGenerator(new Random(5));
		var progress = new Progress();
		foreach (var letter in "ABCDE")
		{
			progress.RecordAnswer(letter, true);
			progress.RecordAnswer(letter, true);
		}

		var asked = Enumerable.Range(0, 3000)
			.Select(_ => generator.Create(ExerciseType.WriteBraille, progress).Letter)
			.ToArray();

		Assert.True(asked.Count(letter => letter == 'F') > asked.Count(letter => letter == 'A') * 2);
	}

	[Fact]
	public void Word_questions_use_known_letters_when_possible()
	{
		var generator = new QuestionGenerator(new Random(6));
		var progress = new Progress();

		for (var i = 0; i < 100; i++)
		{
			var question = generator.Create(ExerciseType.ConvertWord, progress);

			Assert.Contains(question.Target, WordBank.Words);
			Assert.All(question.Target, letter => Assert.Contains(letter, progress.UnlockedLetters));
		}
	}
}
