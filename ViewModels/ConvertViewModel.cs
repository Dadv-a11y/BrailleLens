using BrailleLens.Core.Braille;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class ConvertViewModel : ObservableObject, IQueryAttributable
{
	public const string TextToBrailleTitle = "Texte → Braille";
	public const string BrailleToTextTitle = "Braille → Texte";
	public const string BrailleParameter = "braille";

	private readonly SettingsService _settings;
	private readonly List<BrailleCellModel> _cells = new();

	public ConvertViewModel(SettingsService settings)
	{
		_settings = settings;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (!query.TryGetValue(BrailleParameter, out var value) || value is not string unicode)
		{
			return;
		}

		_cells.Clear();
		foreach (var symbol in unicode)
		{
			_cells.Add(BrailleCellModel.TryFromUnicode(symbol, out var cell) ? cell : BrailleCellModel.Empty);
		}
		EditorCell = BrailleCellModel.Empty;
		IsTextToBraille = false;
		UpdateComposed();
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsBrailleToText), nameof(ModeTitle))]
	private bool _isTextToBraille = true;

	[ObservableProperty]
	private bool _isModeListOpen;

	[ObservableProperty]
	private string _inputText = string.Empty;

	[ObservableProperty]
	private BrailleTranslation _translation = BrailleTranslator.ToBraille(string.Empty);

	[ObservableProperty]
	private string _translationSummary = string.Empty;

	[ObservableProperty]
	private bool _isTranslationEmpty = true;

	[ObservableProperty]
	private string _unsupportedText = string.Empty;

	[ObservableProperty]
	private bool _hasUnsupported;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EditorHint))]
	private BrailleCellModel _editorCell;

	[ObservableProperty]
	private BrailleTranslation _composedTranslation = BrailleTranslator.FromCells([]);

	[ObservableProperty]
	private string _decodedText = string.Empty;

	[ObservableProperty]
	private string _composedSummary = string.Empty;

	[ObservableProperty]
	private double _dotSize = 48;

	[ObservableProperty]
	private bool _showDotNumbers = true;

	public bool IsBrailleToText => !IsTextToBraille;

	public string ModeTitle => IsTextToBraille ? TextToBrailleTitle : BrailleToTextTitle;

	public string EditorHint => $"Cellule sélectionnée · {BrailleDescriber.DescribeDots(EditorCell)}";

	public bool HasCells => _cells.Count > 0;

	public void Load()
	{
		DotSize = _settings.LargeDotSize;
		ShowDotNumbers = _settings.ShowDotNumbers;
		UpdateTranslation();
		UpdateComposed();
	}

	partial void OnInputTextChanged(string value) => UpdateTranslation();

	private void UpdateTranslation()
	{
		Translation = BrailleTranslator.ToBraille(InputText);
		var letters = Translation.LetterCount;
		var cells = Translation.CellCount;
		TranslationSummary = $"{Plural(letters, "lettre")} · {Plural(cells, "cellule")}";
		IsTranslationEmpty = cells == 0;
		HasUnsupported = Translation.HasUnsupported;
		UnsupportedText = HasUnsupported
			? $"Non pris en charge : {string.Join(" ", Translation.UnsupportedCharacters)}"
			: string.Empty;
		CopyBrailleCommand.NotifyCanExecuteChanged();
		ShareBrailleCommand.NotifyCanExecuteChanged();
	}

	private void UpdateComposed()
	{
		ComposedTranslation = BrailleTranslator.FromCells(_cells);
		DecodedText = BrailleTranslator.ToText(_cells);
		ComposedSummary = _cells.Count == 0
			? "Aucune cellule"
			: $"{Plural(_cells.Count, "cellule")} · {DecodedText.Trim()}";
		OnPropertyChanged(nameof(HasCells));
		CopyTextCommand.NotifyCanExecuteChanged();
		ClearCommand.NotifyCanExecuteChanged();
		RemoveLastCellCommand.NotifyCanExecuteChanged();
	}

	private static string Plural(int count, string word) => $"{count} {word}{(count > 1 ? "s" : string.Empty)}";

	[RelayCommand]
	private void ToggleModeList() => IsModeListOpen = !IsModeListOpen;

	[RelayCommand]
	private void SelectTextToBraille() => SetMode(true);

	[RelayCommand]
	private void SelectBrailleToText() => SetMode(false);

	private void SetMode(bool textToBraille)
	{
		IsModeListOpen = false;
		if (IsTextToBraille == textToBraille)
		{
			return;
		}

		if (textToBraille)
		{
			if (_cells.Count > 0)
			{
				InputText = DecodedText;
			}
		}
		else if (Translation.CellCount > 0)
		{
			_cells.Clear();
			_cells.AddRange(Translation.Tokens.Where(token => token.HasCell).Select(token => token.Cell));
			EditorCell = BrailleCellModel.Empty;
			UpdateComposed();
		}

		IsTextToBraille = textToBraille;
		SemanticScreenReader.Announce($"Mode {ModeTitle}");
	}

	private bool HasBraille() => Translation.CellCount > 0;

	[RelayCommand(CanExecute = nameof(HasBraille))]
	private async Task CopyBraille()
	{
		await Clipboard.Default.SetTextAsync(Translation.ToUnicode());
		SemanticScreenReader.Announce("Braille copié.");
	}

	[RelayCommand(CanExecute = nameof(HasBraille))]
	private Task ShareBraille() =>
		Share.Default.RequestAsync(new ShareTextRequest
		{
			Title = "Partager le braille",
			Text = Translation.ToUnicode(),
		});

	[RelayCommand]
	private void ValidateCell()
	{
		_cells.Add(EditorCell);
		var added = EditorCell.IsEmpty ? "Espace ajouté." : $"{BrailleTranslator.ToLetter(EditorCell)} ajouté.";
		EditorCell = BrailleCellModel.Empty;
		UpdateComposed();
		SemanticScreenReader.Announce(added);
	}

	[RelayCommand(CanExecute = nameof(HasCells))]
	private void RemoveLastCell()
	{
		_cells.RemoveAt(_cells.Count - 1);
		UpdateComposed();
		SemanticScreenReader.Announce("Dernière cellule retirée.");
	}

	[RelayCommand(CanExecute = nameof(HasCells))]
	private void Clear()
	{
		_cells.Clear();
		EditorCell = BrailleCellModel.Empty;
		UpdateComposed();
		SemanticScreenReader.Announce("Texte effacé.");
	}

	[RelayCommand(CanExecute = nameof(HasCells))]
	private async Task CopyText()
	{
		await Clipboard.Default.SetTextAsync(DecodedText);
		SemanticScreenReader.Announce("Texte copié.");
	}
}
