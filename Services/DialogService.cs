namespace BrailleLens.Services;

public sealed class DialogService
{
	private static Page CurrentPage =>
		Shell.Current?.CurrentPage ?? throw new InvalidOperationException("Aucune page n'est affichée.");

	public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel = "Annuler") =>
		CurrentPage.DisplayAlertAsync(title, message, accept, cancel);

	public Task<string> ChooseAsync(string title, params string[] options) =>
		CurrentPage.DisplayActionSheetAsync(title, "Annuler", null, options);
}
