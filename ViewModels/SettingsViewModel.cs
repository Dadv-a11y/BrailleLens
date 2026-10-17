using BrailleLens.Core.Braille;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrailleLens.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	private readonly SettingsService _settings;
	private bool _isLoading;

	public SettingsViewModel(SettingsService settings)
	{
		_settings = settings;
	}

	[ObservableProperty]
	private bool _showDotNumbers;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PreviewDotSize))]
	private bool _useLargeCells;

	[ObservableProperty]
	private bool _hapticFeedback;

	[ObservableProperty]
	private BrailleCellModel _previewCell = BrailleAlphabet.GetCell('B');

	public double PreviewDotSize => UseLargeCells ? 60 : 48;

	public void Load()
	{
		_isLoading = true;
		ShowDotNumbers = _settings.ShowDotNumbers;
		UseLargeCells = _settings.CellSize == CellDisplaySize.Large;
		HapticFeedback = _settings.HapticFeedback;
		_isLoading = false;
	}

	partial void OnShowDotNumbersChanged(bool value)
	{
		if (!_isLoading)
		{
			_settings.ShowDotNumbers = value;
		}
	}

	partial void OnUseLargeCellsChanged(bool value)
	{
		if (!_isLoading)
		{
			_settings.CellSize = value ? CellDisplaySize.Large : CellDisplaySize.Normal;
		}
	}

	partial void OnHapticFeedbackChanged(bool value)
	{
		if (!_isLoading)
		{
			_settings.HapticFeedback = value;
		}
	}
}
