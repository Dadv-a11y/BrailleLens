using BrailleLens.Core.Braille;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class HomeViewModel : ObservableObject
{
	private readonly ProgressService _progress;
	private readonly NavigationService _navigation;
	private readonly DialogService _dialogs;

	public HomeViewModel(ProgressService progress, NavigationService navigation, DialogService dialogs)
	{
		_progress = progress;
		_navigation = navigation;
		_dialogs = dialogs;
		_progress.Changed += (_, _) => Refresh();
		Refresh();
	}

	public int TotalLetters => BrailleAlphabet.Letters.Count;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsFirstUse))]
	private bool _hasStarted;

	[ObservableProperty]
	private int _masteredCount;

	[ObservableProperty]
	private string _levelText = string.Empty;

	[ObservableProperty]
	private string _masteredText = string.Empty;

	public bool IsFirstUse => !HasStarted;

	public void Refresh()
	{
		var progress = _progress.Progress;
		HasStarted = progress.HasStarted;
		MasteredCount = progress.MasteredCount;
		LevelText = $"Niveau {progress.Level}";
		MasteredText = $"{progress.MasteredCount} / {TotalLetters} lettres maîtrisées";
	}

	[RelayCommand]
	private Task StartLearning()
	{
		_progress.Progress.Start();
		_progress.Save();
		return _navigation.GoToAsync(Routes.Learn);
	}

	[RelayCommand]
	private Task ContinueLearning() => _navigation.GoToAsync(Routes.Learn);

	[RelayCommand]
	private Task OpenConvert() => _navigation.GoToAsync(Routes.Convert);

	[RelayCommand]
	private Task OpenScan() => _navigation.GoToAsync(Routes.Scan);

	[RelayCommand]
	private void OpenMenu() => _navigation.OpenMenu();

	[RelayCommand]
	private async Task ShowMore()
	{
		var choice = await _dialogs.ChooseAsync("Plus", "Aide", "À propos");
		var route = choice switch
		{
			"Aide" => Routes.Help,
			"À propos" => Routes.About,
			_ => null
		};
		if (route is not null)
		{
			await _navigation.GoToAsync(route);
		}
	}
}
