using BrailleLens.Core.Braille;

namespace BrailleLens.Core.Training;

public sealed class Question
{
	public Question(ExerciseType type, string target, IReadOnlyList<char>? choices = null)
	{
		if (string.IsNullOrEmpty(target) || !target.All(BrailleAlphabet.IsSupported))
		{
			throw new ArgumentException("La cible doit être composée de lettres prises en charge.", nameof(target));
		}

		Type = type;
		Target = target.ToUpperInvariant();
		Choices = choices?.Select(char.ToUpperInvariant).ToArray() ?? Array.Empty<char>();
		ExpectedCells = Target.Select(BrailleAlphabet.GetCell).ToArray();

		if (HasChoices && !Choices.Contains(Letter))
		{
			throw new ArgumentException("Les choix doivent contenir la bonne réponse.", nameof(choices));
		}
	}

	public ExerciseType Type { get; }

	public string Target { get; }

	public char Letter => Target[0];

	public IReadOnlyList<char> Choices { get; }

	public IReadOnlyList<BrailleCellModel> ExpectedCells { get; }

	public bool HasChoices => Type is ExerciseType.RecognizeLetter or ExerciseType.FindBraille;

	public bool IsCorrect(char choice) => HasChoices && char.ToUpperInvariant(choice) == Letter;

	public bool IsCorrect(BrailleCellModel cell) => Type == ExerciseType.WriteBraille && cell == ExpectedCells[0];

	public bool IsCorrect(IReadOnlyList<BrailleCellModel> cells) =>
		Type == ExerciseType.ConvertWord && cells.SequenceEqual(ExpectedCells);
}
