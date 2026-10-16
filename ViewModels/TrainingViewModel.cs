using BrailleLens.Controls;
using BrailleLens.Core.Braille;
using BrailleLens.Core.Training;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class TrainingViewModel : ObservableObject
{
	private const string CorrectFeedback = "Bonne réponse !";
	private const string WrongFeedback = "Pas tout à fait.";

	private readonly ProgressService _progress;
	private readonly SettingsService _settings;
	private readonly NavigationService _navigation;
	private readonly List<BrailleCellModel> _composedCells = new();
	private TrainingSession? _session;
	private ExerciseType? _pendingType;

	public TrainingViewModel(ProgressService progress, SettingsService settings, NavigationService navigation)
	{
		_progress = progress;
		_settings = settings;
		_navigation = navigation;
		Exercises = ExerciseOption.CreateAll();
		Choices = Enumerable.Range(0, QuestionGenerator.ChoiceCount).Select(index => new ChoiceItem(index)).ToArray();
	}

	public IReadOnlyList<ExerciseOption> Exercises { get; }

	public IReadOnlyList<ChoiceItem> Choices { get; }

	public int QuestionCount => TrainingSession.DefaultQuestionCount;

	public int TotalLetters => BrailleAlphabet.Letters.Count;

	[ObservableProperty]
	private ExerciseType _exerciseType = ExerciseType.RecognizeLetter;

	[ObservableProperty]
	private int _questionNumber = 1;

	[ObservableProperty]
	private string _levelText = string.Empty;

	[ObservableProperty]
	private string _target = string.Empty;

	[ObservableProperty]
	private BrailleCellModel _questionCell;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(VerifyCommand), nameof(AddCellCommand))]
	private BrailleCellModel _editorCell;

	[ObservableProperty]
	private BrailleTranslation _composedTranslation = BrailleTranslator.FromCells([]);

	[ObservableProperty]
	private BrailleTranslation _expectedTranslation = BrailleTranslator.FromCells([]);

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsAwaitingAnswer), nameof(ShowChoiceHint), nameof(ShowVerify), nameof(ShowExpected), nameof(ShowAddCell), nameof(ShowContinue))]
	[NotifyCanExecuteChangedFor(nameof(VerifyCommand), nameof(AddCellCommand), nameof(RemoveLastCellCommand), nameof(ChooseCommand))]
	private bool _isAnswered;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ShowExpected))]
	private bool _isCorrect;

	[ObservableProperty]
	private string _feedbackText = string.Empty;

	[ObservableProperty]
	private bool _isPickerOpen;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsQuizVisible), nameof(ShowContinue))]
	private bool _isFinished;

	[ObservableProperty]
	private string _scoreText = string.Empty;

	[ObservableProperty]
	private string _resultTitle = string.Empty;

	[ObservableProperty]
	private string _resultMasteredText = string.Empty;

	[ObservableProperty]
	private int _resultMasteredCount;

	[ObservableProperty]
	private string _gainedText = string.Empty;

	[ObservableProperty]
	private bool _hasGained;

	[ObservableProperty]
	private double _dotSize = 48;

	[ObservableProperty]
	private bool _showDotNumbers = true;

	public string QuestionText => $"Question {QuestionNumber} / {QuestionCount}";

	public string ExerciseTitle => Exercises.First(option => option.Type == ExerciseType).Title;

	public bool IsRecognizeLetter => ExerciseType == ExerciseType.RecognizeLetter;

	public bool IsFindBraille => ExerciseType == ExerciseType.FindBraille;

	public bool IsWriteBraille => ExerciseType == ExerciseType.WriteBraille;

	public bool IsConvertWord => ExerciseType == ExerciseType.ConvertWord;

	public bool HasChoices => IsRecognizeLetter || IsFindBraille;

	public bool ShowTargetLetter => IsFindBraille || IsWriteBraille;

	public bool IsAwaitingAnswer => !IsAnswered;

	public bool ShowChoiceHint => HasChoices && !IsAnswered;

	public bool ShowVerify => !HasChoices && !IsAnswered;

	public bool ShowExpected => IsAnswered && !IsCorrect && !HasChoices;

	public bool ShowEditor => !HasChoices;

	public bool ShowAddCell => IsConvertWord && !IsAnswered;

	public bool ShowContinue => IsAnswered && !IsFinished;

	public bool IsQuizVisible => !IsFinished;

	public string Prompt => ExerciseType switch
	{
		ExerciseType.RecognizeLetter => "Quelle lettre représente cette cellule ?",
		ExerciseType.FindBraille => "Quelle cellule correspond à cette lettre ?",
		ExerciseType.WriteBraille => "Composez cette lettre en braille.",
		_ => "Convertissez ce mot en braille.",
	};

	public string ComposedText => $"{_composedCells.Count} / {Target.Length} cellules";

	public string EditorHint => $"Cellule en cours · {BrailleDescriber.DescribeDots(EditorCell)}";

	public void Load()
	{
		DotSize = _settings.LargeDotSize;
		ShowDotNumbers = _settings.ShowDotNumbers;
		if (_session is null)
		{
			_session = new TrainingSession(_progress.Progress, new QuestionGenerator(), ExerciseType);
			ShowQuestion();
		}
		LevelText = $"Niveau {_progress.Progress.Level}";
	}

	partial void OnExerciseTypeChanged(ExerciseType value)
	{
		foreach (var option in Exercises)
		{
			option.IsSelected = option.Type == value;
		}
		OnPropertyChanged(nameof(ExerciseTitle));
		OnPropertyChanged(nameof(IsRecognizeLetter));
		OnPropertyChanged(nameof(IsFindBraille));
		OnPropertyChanged(nameof(IsWriteBraille));
		OnPropertyChanged(nameof(IsConvertWord));
		OnPropertyChanged(nameof(HasChoices));
		OnPropertyChanged(nameof(ShowTargetLetter));
		OnPropertyChanged(nameof(ShowChoiceHint));
		OnPropertyChanged(nameof(ShowVerify));
		OnPropertyChanged(nameof(ShowExpected));
		OnPropertyChanged(nameof(ShowEditor));
		OnPropertyChanged(nameof(ShowAddCell));
		OnPropertyChanged(nameof(Prompt));
	}

	partial void OnQuestionNumberChanged(int value) => OnPropertyChanged(nameof(QuestionText));

	partial void OnEditorCellChanged(BrailleCellModel value) => OnPropertyChanged(nameof(EditorHint));

	partial void OnTargetChanged(string value) => OnPropertyChanged(nameof(ComposedText));

	private void ShowQuestion()
	{
		var session = _session!;
		var question = session.CurrentQuestion;

		ExerciseType = session.Type;
		QuestionNumber = session.QuestionNumber;
		Target = question.Target;
		QuestionCell = question.ExpectedCells[0];
		ExpectedTranslation = BrailleTranslator.ToBraille(question.Target);

		for (var i = 0; i < Choices.Count; i++)
		{
			Choices[i].Letter = i < question.Choices.Count ? question.Choices[i] : default;
			Choices[i].State = ChoiceState.Normal;
		}

		_composedCells.Clear();
		UpdateComposed();
		EditorCell = BrailleCellModel.Empty;
		FeedbackText = string.Empty;
		IsCorrect = false;
		IsAnswered = false;
		OnExerciseTypeChanged(ExerciseType);
	}

	private void UpdateComposed()
	{
		ComposedTranslation = BrailleTranslator.FromCells(_composedCells);
		OnPropertyChanged(nameof(ComposedText));
		AddCellCommand.NotifyCanExecuteChanged();
		VerifyCommand.NotifyCanExecuteChanged();
		RemoveLastCellCommand.NotifyCanExecuteChanged();
	}

	private bool CanChoose(ChoiceItem? item) => !IsAnswered;

	[RelayCommand(CanExecute = nameof(CanChoose))]
	private void Choose(ChoiceItem? chosen)
	{
		if (chosen is null)
		{
			return;
		}

		var session = _session!;
		var correct = session.Answer(chosen.Letter);

		foreach (var choice in Choices)
		{
			choice.State = choice.Letter == session.CurrentQuestion.Letter
				? ChoiceState.Correct
				: choice == chosen ? ChoiceState.Wrong : ChoiceState.Normal;
		}
		CompleteAnswer(correct);
	}

	private List<BrailleCellModel> CellsToCheck()
	{
		var cells = new List<BrailleCellModel>(_composedCells);
		if (!EditorCell.IsEmpty)
		{
			cells.Add(EditorCell);
		}
		return cells;
	}

	private bool CanVerify() =>
		!IsAnswered && (IsWriteBraille ? !EditorCell.IsEmpty : CellsToCheck().Count == Target.Length);

	[RelayCommand(CanExecute = nameof(CanVerify))]
	private void Verify()
	{
		var session = _session!;
		bool correct;
		if (IsWriteBraille)
		{
			correct = session.Answer(EditorCell);
		}
		else
		{
			var cells = CellsToCheck();
			_composedCells.Clear();
			_composedCells.AddRange(cells);
			EditorCell = BrailleCellModel.Empty;
			UpdateComposed();
			correct = session.Answer(cells);
		}
		CompleteAnswer(correct);
	}

	private bool CanAddCell() => !IsAnswered && IsConvertWord && !EditorCell.IsEmpty && _composedCells.Count < Target.Length - 1;

	[RelayCommand(CanExecute = nameof(CanAddCell))]
	private void AddCell()
	{
		_composedCells.Add(EditorCell);
		EditorCell = BrailleCellModel.Empty;
		UpdateComposed();
		SemanticScreenReader.Announce($"Cellule ajoutée. {ComposedText}.");
	}

	private bool CanRemoveLastCell() => !IsAnswered && _composedCells.Count > 0;

	[RelayCommand(CanExecute = nameof(CanRemoveLastCell))]
	private void RemoveLastCell()
	{
		_composedCells.RemoveAt(_composedCells.Count - 1);
		UpdateComposed();
		SemanticScreenReader.Announce($"Dernière cellule retirée. {ComposedText}.");
	}

	private void CompleteAnswer(bool correct)
	{
		_progress.Save();
		LevelText = $"Niveau {_progress.Progress.Level}";
		IsCorrect = correct;
		FeedbackText = correct ? CorrectFeedback : WrongFeedback;
		IsAnswered = true;
		SemanticScreenReader.Announce(correct ? FeedbackText : $"{FeedbackText} Réponse attendue : {Target}.");
	}

	[RelayCommand]
	private void Continue()
	{
		var session = _session!;
		session.Next();
		if (session.IsFinished)
		{
			ShowResult();
			return;
		}

		if (_pendingType is { } type)
		{
			_pendingType = null;
			session.ChangeExercise(type);
		}
		ShowQuestion();
	}

	private void ShowResult()
	{
		var result = _session!.GetResult();
		ScoreText = $"{result.Score} / {result.QuestionCount}";
		ResultTitle = result.Score >= 8 ? "Bravo !" : result.Score >= 5 ? "Bien joué !" : "Continue comme ça !";
		LevelText = $"Niveau {result.Level}";
		ResultMasteredCount = result.MasteredAfter;
		ResultMasteredText = $"{result.MasteredAfter} / {TotalLetters} lettres maîtrisées";
		HasGained = result.GainedLetters > 0;
		GainedText = result.GainedLetters == 1 ? "+1 lettre maîtrisée" : $"+{result.GainedLetters} lettres maîtrisées";
		IsFinished = true;
		SemanticScreenReader.Announce($"{ResultTitle} {result.Score} bonnes réponses sur {result.QuestionCount}.");
	}

	[RelayCommand]
	private void Restart()
	{
		_pendingType = null;
		_session!.Restart();
		IsFinished = false;
		LevelText = $"Niveau {_progress.Progress.Level}";
		ShowQuestion();
	}

	[RelayCommand]
	private Task Finish() => _navigation.GoBackAsync();

	[RelayCommand]
	private void OpenPicker() => IsPickerOpen = true;

	[RelayCommand]
	private void ClosePicker() => IsPickerOpen = false;

	[RelayCommand]
	private void SelectExercise(ExerciseType type)
	{
		IsPickerOpen = false;
		var session = _session!;
		if (type == session.Type && _pendingType is null)
		{
			return;
		}

		if (session.IsAnswered)
		{
			_pendingType = type;
			Continue();
			return;
		}

		session.ChangeExercise(type);
		ShowQuestion();
	}
}
