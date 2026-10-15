using BrailleLens.Core.Braille;
using BrailleLens.Core.Learning;

namespace BrailleLens.Core.Training;

public sealed class QuestionGenerator
{
	public const int ChoiceCount = 4;

	private const int DistractorPoolSize = 6;
	private const int UnmasteredWeight = 3;

	private readonly Random _random;

	public QuestionGenerator(Random? random = null)
	{
		_random = random ?? Random.Shared;
	}

	public Question Create(ExerciseType type, Progress progress, string? previousTarget = null)
	{
		if (type == ExerciseType.ConvertWord)
		{
			return new Question(type, WordBank.Pick(progress.UnlockedLetters, _random, previousTarget));
		}

		var letter = PickLetter(progress, previousTarget);
		return type == ExerciseType.WriteBraille
			? new Question(type, letter.ToString())
			: new Question(type, letter.ToString(), CreateChoices(letter));
	}

	private char PickLetter(Progress progress, string? previousTarget)
	{
		var pool = progress.UnlockedLetters
			.Where(letter => previousTarget is not { Length: 1 } || letter != previousTarget[0])
			.ToArray();
		var weights = pool.Select(letter => progress.IsMastered(letter) ? 1 : UnmasteredWeight).ToArray();

		var roll = _random.Next(weights.Sum());
		for (var i = 0; i < pool.Length; i++)
		{
			roll -= weights[i];
			if (roll < 0)
			{
				return pool[i];
			}
		}
		return pool[^1];
	}

	private IReadOnlyList<char> CreateChoices(char letter)
	{
		var target = BrailleAlphabet.GetCell(letter);
		var distractors = BrailleAlphabet.Letters
			.Where(other => other != letter)
			.OrderBy(other => BrailleAlphabet.GetCell(other).DistanceTo(target))
			.ThenBy(_ => _random.Next())
			.Take(DistractorPoolSize)
			.OrderBy(_ => _random.Next())
			.Take(ChoiceCount - 1);

		return distractors.Append(letter).OrderBy(_ => _random.Next()).ToArray();
	}
}
