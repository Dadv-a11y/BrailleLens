namespace BrailleLens.Services;

public enum CellDisplaySize
{
	Normal,
	Large
}

public sealed class SettingsService
{
	private const string ShowDotNumbersKey = "settings.showDotNumbers";
	private const string CellSizeKey = "settings.cellSize";
	private const string HapticFeedbackKey = "settings.haptics";

	private readonly IPreferences _preferences;

	public SettingsService(IPreferences preferences)
	{
		_preferences = preferences;
		Haptics.IsEnabled = HapticFeedback;
	}

	public event EventHandler? Changed;

	public bool ShowDotNumbers
	{
		get => _preferences.Get(ShowDotNumbersKey, true);
		set => Set(ShowDotNumbersKey, value);
	}

	public CellDisplaySize CellSize
	{
		get => (CellDisplaySize)_preferences.Get(CellSizeKey, (int)CellDisplaySize.Normal);
		set => Set(CellSizeKey, (int)value);
	}

	public bool HapticFeedback
	{
		get => _preferences.Get(HapticFeedbackKey, true);
		set
		{
			Haptics.IsEnabled = value;
			Set(HapticFeedbackKey, value);
		}
	}

	public double LargeDotSize => CellSize == CellDisplaySize.Large ? 60 : 48;

	private void Set<T>(string key, T value)
	{
		_preferences.Set(key, value);
		Changed?.Invoke(this, EventArgs.Empty);
	}
}
