using BrailleLens.Services;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class ShellViewModel
{
	private readonly NavigationService _navigation;
	private readonly DialogService _dialogs;
	private readonly ProgressService _progress;

	public ShellViewModel(NavigationService navigation, DialogService dialogs, ProgressService progress)
	{
		_navigation = navigation;
		_dialogs = dialogs;
		_progress = progress;
	}

	[RelayCommand]
	private void CloseMenu() => _navigation.CloseMenu();

	[RelayCommand]
	private Task OpenSettings() => OpenFromMenu(Routes.Settings);

	[RelayCommand]
	private Task OpenHelp() => OpenFromMenu(Routes.Help);

	[RelayCommand]
	private Task OpenAbout() => OpenFromMenu(Routes.About);

	[RelayCommand]
	private async Task ResetProgress()
	{
		_navigation.CloseMenu();
		var confirmed = await _dialogs.ConfirmAsync(
			"Réinitialiser la progression",
			"Toutes les lettres maîtrisées et ton niveau seront effacés. Cette action est définitive.",
			"Réinitialiser");
		if (!confirmed)
		{
			return;
		}

		_progress.Reset();
		await _navigation.GoHomeAsync();
		SemanticScreenReader.Announce("Progression réinitialisée.");
	}

	private async Task OpenFromMenu(string route)
	{
		_navigation.CloseMenu();
		await _navigation.GoToAsync(route);
	}
}
