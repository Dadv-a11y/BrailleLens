using System.Collections.ObjectModel;
using BrailleLens.Core.Braille;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class LearnViewModel : ObservableObject
{
	public const string DefaultHint = "Touchez un point pour explorer la cellule.";

	private readonly ProgressService _progress;
	private readonly SettingsService _settings;
	private readonly NavigationService _navigation;

	public LearnViewModel(ProgressService progress, SettingsService settings, NavigationService navigation)
	{
		_progress = progress;
		_settings = settings;
		_navigation = navigation;
		Letters = new ObservableCollection<LetterItem>(BrailleAlphabet.Letters.Select(letter => new LetterItem(letter)));
	}

	public ObservableCollection<LetterItem> Letters { get; }

	public int TotalLetters => BrailleAlphabet.Letters.Count;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LetterText), nameof(Cell), nameof(Explanation), nameof(SelectedLegend), nameof(IsCurrentMastered))]
	[NotifyCanExecuteChangedFor(nameof(PreviousCommand), nameof(NextCommand))]
	private char _currentLetter = 'A';

	[ObservableProperty]
	private string _hint = DefaultHint;

	[ObservableProperty]
	private int _masteredCount;

	[ObservableProperty]
	private string _masteredText = string.Empty;

	[ObservableProperty]
	private string _counterText = string.Empty;

	[ObservableProperty]
	private string _levelText = string.Empty;

	[ObservableProperty]
	private bool _isPickerOpen;

	[ObservableProperty]
	private double _dotSize = 48;

	[ObservableProperty]
	private bool _showDotNumbers = true;

	public string LetterText => CurrentLetter.ToString();

	public BrailleCellModel Cell => BrailleAlphabet.GetCell(CurrentLetter);

	public string Explanation => BrailleDescriber.DescribeLetter(CurrentLetter);

	public bool IsCurrentMastered => _progress.Progress.IsMastered(CurrentLetter);

	public string SelectedLegend =>
		IsCurrentMastered
			? $"Maîtrisée · {CurrentLetter} sélectionnée"
			: $"{CurrentLetter} sélectionnée";

	public string CounterDescription => $"Choisir une lettre. {MasteredText}";

	public void Load()
	{
		var progress = _progress.Progress;
		if (!progress.HasStarted)
		{
			progress.Start();
			_progress.Save();
		}

		DotSize = _settings.LargeDotSize;
		ShowDotNumbers = _settings.ShowDotNumbers;
		CurrentLetter = progress.CurrentLetter;
		RefreshProgress();
	}

	partial void OnCurrentLetterChanged(char value)
	{
		Hint = DefaultHint;
		foreach (var item in Letters)
		{
			item.IsSelected = item.Letter == value;
		}

		if (_progress.Progress.CurrentLetter != value)
		{
			_progress.Progress.SelectLetter(value);
			_progress.Save();
		}
	}

	private void RefreshProgress()
	{
		var progress = _progress.Progress;
		MasteredCount = progress.MasteredCount;
		MasteredText = $"{progress.MasteredCount} / {TotalLetters} lettres maîtrisées";
		CounterText = $"{progress.MasteredCount} / {TotalLetters}";
		LevelText = $"Niveau {progress.Level}";
		foreach (var item in Letters)
		{
			item.IsMastered = progress.IsMastered(item.Letter);
			item.IsSelected = item.Letter == CurrentLetter;
		}
		OnPropertyChanged(nameof(SelectedLegend));
		OnPropertyChanged(nameof(IsCurrentMastered));
		OnPropertyChanged(nameof(CounterDescription));
	}

	private bool CanGoPrevious() => BrailleAlphabet.IndexOf(CurrentLetter) > 0;

	private bool CanGoNext() => BrailleAlphabet.IndexOf(CurrentLetter) < TotalLetters - 1;

	[RelayCommand(CanExecute = nameof(CanGoPrevious))]
	private void Previous() => MoveBy(-1);

	[RelayCommand(CanExecute = nameof(CanGoNext))]
	private void Next() => MoveBy(1);

	[RelayCommand]
	private void ExploreDot(int dot)
	{
		Hint = $"Point {dot} : {(Cell.IsRaised(dot) ? "levé" : "non levé")}.";
		SemanticScreenReader.Announce(Hint);
	}

	[RelayCommand]
	private void OpenPicker() => IsPickerOpen = true;

	[RelayCommand]
	private void ClosePicker() => IsPickerOpen = false;

	[RelayCommand]
	private void SelectLetter(char letter)
	{
		CurrentLetter = letter;
		SemanticScreenReader.Announce(Explanation);
	}

	[RelayCommand]
	private Task OpenTraining() => _navigation.GoToAsync(Routes.Training);

	private void MoveBy(int offset)
	{
		var index = Math.Clamp(BrailleAlphabet.IndexOf(CurrentLetter) + offset, 0, TotalLetters - 1);
		CurrentLetter = BrailleAlphabet.Letters[index];
		SemanticScreenReader.Announce($"{CurrentLetter}. {Explanation}");
	}
}
