using BrailleLens.Core.Braille;
using BrailleLens.Core.Learning;

namespace BrailleLens.Core.Training;

public sealed record SessionResult(int Score, int QuestionCount, int MasteredBefore, int MasteredAfter, int Level)
{
	public int GainedLetters => Math.Max(0, MasteredAfter - MasteredBefore);
}

public sealed class TrainingSession
{
	public const int DefaultQuestionCount = 10;

	private readonly Progress _progress;
	private readonly QuestionGenerator _generator;
	private int _masteredBefore;

	public TrainingSession(
		Progress progress,
		QuestionGenerator generator,
		ExerciseType type = ExerciseType.RecognizeLetter,
		int questionCount = DefaultQuestionCount)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(questionCount, 1);

		_progress = progress;
		_generator = generator;
		Type = type;
		QuestionCount = questionCount;
		Restart();
	}

	public ExerciseType Type { get; private set; }

	public int QuestionCount { get; }

	public int QuestionNumber { get; private set; }

	public int Score { get; private set; }

	public Question CurrentQuestion { get; private set; } = null!;

	public bool IsAnswered { get; private set; }

	public bool? LastAnswerCorrect { get; private set; }

	public bool IsFinished { get; private set; }

	public bool IsLastQuestion => QuestionNumber == QuestionCount;

	public void Restart()
	{
		_masteredBefore = _progress.MasteredCount;
		Score = 0;
		QuestionNumber = 1;
		IsFinished = false;
		LoadQuestion(previousTarget: null);
	}

	public void ChangeExercise(ExerciseType type)
	{
		EnsureAwaitingAnswer();
		if (type == Type)
		{
			return;
		}
		Type = type;
		LoadQuestion(CurrentQuestion.Target);
	}

	public bool Answer(char choice)
	{
		EnsureAwaitingAnswer(ExerciseType.RecognizeLetter, ExerciseType.FindBraille);
		var correct = CurrentQuestion.IsCorrect(choice);
		_progress.RecordAnswer(CurrentQuestion.Letter, correct);
		return Complete(correct);
	}

	public bool Answer(BrailleCellModel cell)
	{
		EnsureAwaitingAnswer(ExerciseType.WriteBraille);
		var correct = CurrentQuestion.IsCorrect(cell);
		_progress.RecordAnswer(CurrentQuestion.Letter, correct);
		return Complete(correct);
	}

	public bool Answer(IReadOnlyList<BrailleCellModel> cells)
	{
		EnsureAwaitingAnswer(ExerciseType.ConvertWord);
		var expected = CurrentQuestion.ExpectedCells;
		for (var i = 0; i < expected.Count; i++)
		{
			_progress.RecordAnswer(CurrentQuestion.Target[i], i < cells.Count && cells[i] == expected[i]);
		}
		return Complete(CurrentQuestion.IsCorrect(cells));
	}

	public void Next()
	{
		if (!IsAnswered || IsFinished)
		{
			throw new InvalidOperationException("La question en cours n'a pas encore reçu de réponse.");
		}

		if (IsLastQuestion)
		{
			IsFinished = true;
			return;
		}

		QuestionNumber++;
		LoadQuestion(CurrentQuestion.Target);
	}

	public SessionResult GetResult() =>
		new(Score, QuestionCount, _masteredBefore, _progress.MasteredCount, _progress.Level);

	private bool Complete(bool correct)
	{
		if (correct)
		{
			Score++;
		}
		IsAnswered = true;
		LastAnswerCorrect = correct;
		return correct;
	}

	private void LoadQuestion(string? previousTarget)
	{
		CurrentQuestion = _generator.Create(Type, _progress, previousTarget);
		IsAnswered = false;
		LastAnswerCorrect = null;
	}

	private void EnsureAwaitingAnswer(params ExerciseType[] allowedTypes)
	{
		if (IsFinished || IsAnswered)
		{
			throw new InvalidOperationException("Aucune question n'attend de réponse.");
		}
		if (allowedTypes.Length > 0 && !allowedTypes.Contains(Type))
		{
			throw new InvalidOperationException($"Ce type de réponse ne correspond pas à l'exercice {Type}.");
		}
	}
}
