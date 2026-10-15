using BrailleLens.Core.Braille;
using BrailleLens.Core.Learning;
using BrailleLens.Core.Training;

namespace BrailleLens.Tests.Training;

public class TrainingSessionTests
{
	[Fact]
	public void Session_starts_at_question_1_of_10()
	{
		var session = CreateSession();

		Assert.Equal(1, session.QuestionNumber);
		Assert.Equal(10, session.QuestionCount);
		Assert.Equal(0, session.Score);
		Assert.False(session.IsAnswered);
		Assert.False(session.IsFinished);
	}

	[Fact]
	public void Correct_answer_increases_score()
	{
		var session = CreateSession();

		Assert.True(session.Answer(session.CurrentQuestion.Letter));
		Assert.Equal(1, session.Score);
		Assert.True(session.IsAnswered);
		Assert.True(session.LastAnswerCorrect);
	}

	[Fact]
	public void Wrong_answer_keeps_score()
	{
		var session = CreateSession();
		var wrong = session.CurrentQuestion.Choices.First(choice => choice != session.CurrentQuestion.Letter);

		Assert.False(session.Answer(wrong));
		Assert.Equal(0, session.Score);
		Assert.False(session.LastAnswerCorrect);
	}

	[Fact]
	public void Cannot_answer_twice_or_skip_unanswered_question()
	{
		var session = CreateSession();

		Assert.Throws<InvalidOperationException>(session.Next);
		session.Answer(session.CurrentQuestion.Letter);
		Assert.Throws<InvalidOperationException>(() => session.Answer(session.CurrentQuestion.Letter));
	}

	[Fact]
	public void Answer_type_must_match_exercise()
	{
		var session = CreateSession(ExerciseType.WriteBraille);

		Assert.Throws<InvalidOperationException>(() => session.Answer('A'));
		Assert.Throws<InvalidOperationException>(() => session.Answer(new[] { BrailleCellModel.Empty }));
	}

	[Fact]
	public void Full_session_finishes_with_result()
	{
		var progress = new Progress();
		var session = CreateSession(progress: progress);

		for (var i = 0; i < 10; i++)
		{
			var question = session.CurrentQuestion;
			if (i < 8)
			{
				session.Answer(question.Letter);
			}
			else
			{
				session.Answer(question.Choices.First(choice => choice != question.Letter));
			}
			session.Next();
		}

		var result = session.GetResult();
		Assert.True(session.IsFinished);
		Assert.Equal(8, result.Score);
		Assert.Equal(10, result.QuestionCount);
		Assert.Equal(0, result.MasteredBefore);
		Assert.Equal(progress.MasteredCount, result.MasteredAfter);
		Assert.Equal(progress.MasteredCount, result.GainedLetters);
		Assert.Equal(progress.Level, result.Level);
	}

	[Fact]
	public void Correct_answers_feed_letter_mastery()
	{
		var progress = new Progress();
		var session = CreateSession(ExerciseType.WriteBraille, progress);
		var letter = session.CurrentQuestion.Letter;

		session.Answer(BrailleAlphabet.GetCell(letter));

		Assert.False(progress.IsMastered(letter));
		progress.RecordAnswer(letter, true);
		Assert.True(progress.IsMastered(letter));
	}

	[Fact]
	public void Word_answer_is_checked_cell_by_cell()
	{
		var progress = new Progress();
		var session = CreateSession(ExerciseType.ConvertWord, progress);
		var question = session.CurrentQuestion;

		Assert.True(session.Answer(question.ExpectedCells));
		Assert.Equal(1, session.Score);
		foreach (var letter in question.Target.Distinct())
		{
			progress.RecordAnswer(letter, true);
			Assert.True(progress.IsMastered(letter));
		}
	}

	[Fact]
	public void Partial_word_answer_is_wrong()
	{
		var session = CreateSession(ExerciseType.ConvertWord);
		var cells = session.CurrentQuestion.ExpectedCells.Take(1).ToArray();

		Assert.False(session.Answer(cells));
		Assert.Equal(0, session.Score);
	}

	[Fact]
	public void ChangeExercise_replaces_current_question_and_keeps_position()
	{
		var session = CreateSession();
		session.Answer(session.CurrentQuestion.Letter);
		session.Next();

		session.ChangeExercise(ExerciseType.ConvertWord);

		Assert.Equal(ExerciseType.ConvertWord, session.Type);
		Assert.Equal(ExerciseType.ConvertWord, session.CurrentQuestion.Type);
		Assert.Equal(2, session.QuestionNumber);
		Assert.Equal(1, session.Score);
	}

	[Fact]
	public void Restart_resets_score_and_counts_gains_from_now()
	{
		var progress = new Progress();
		progress.RecordAnswer('A', true);
		progress.RecordAnswer('A', true);
		var session = CreateSession(progress: progress);
		session.Answer(session.CurrentQuestion.Letter);
		session.Next();

		session.Restart();

		Assert.Equal(1, session.QuestionNumber);
		Assert.Equal(0, session.Score);
		Assert.Equal(progress.MasteredCount, session.GetResult().MasteredBefore);
	}

	private static TrainingSession CreateSession(ExerciseType type = ExerciseType.RecognizeLetter, Progress? progress = null) =>
		new(progress ?? new Progress(), new QuestionGenerator(new Random(42)), type);
}
