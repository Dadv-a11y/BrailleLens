using BrailleLens.Core.Braille;
using BrailleLens.Core.Training;

namespace BrailleLens.Tests.Training;

public class QuestionTests
{
	[Fact]
	public void Choice_question_checks_selected_letter()
	{
		var question = new Question(ExerciseType.RecognizeLetter, "b", ['a', 'b', 'c', 'd']);

		Assert.True(question.IsCorrect('B'));
		Assert.False(question.IsCorrect('C'));
		Assert.False(question.IsCorrect(BrailleAlphabet.GetCell('B')));
	}

	[Fact]
	public void Write_question_checks_composed_cell()
	{
		var question = new Question(ExerciseType.WriteBraille, "B");

		Assert.True(question.IsCorrect(BrailleCellModel.FromDots(1, 2)));
		Assert.False(question.IsCorrect(BrailleCellModel.FromDots(1)));
		Assert.False(question.IsCorrect('B'));
	}

	[Fact]
	public void Word_question_checks_every_cell_in_order()
	{
		var question = new Question(ExerciseType.ConvertWord, "AMI");
		var cells = "AMI".Select(BrailleAlphabet.GetCell).ToArray();

		Assert.True(question.IsCorrect(cells));
		Assert.False(question.IsCorrect(cells.Take(2).ToArray()));
		Assert.False(question.IsCorrect(cells.Reverse().ToArray()));
	}

	[Fact]
	public void Invalid_questions_are_rejected()
	{
		Assert.Throws<ArgumentException>(() => new Question(ExerciseType.ConvertWord, "été"));
		Assert.Throws<ArgumentException>(() => new Question(ExerciseType.FindBraille, "B", ['A', 'C', 'D', 'E']));
	}
}
