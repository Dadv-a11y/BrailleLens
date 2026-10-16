namespace BrailleLens.Services;

public sealed class NavigationService
{
	private static Shell Shell => Shell.Current ?? throw new InvalidOperationException("Le Shell n'est pas encore prêt.");

	public Task GoToAsync(string route, IDictionary<string, object>? parameters = null) =>
		parameters is null ? Shell.GoToAsync(route) : Shell.GoToAsync(route, parameters);

	public Task GoBackAsync() => Shell.GoToAsync("..");

	public Task GoHomeAsync() => Shell.GoToAsync($"//{Routes.Home}");

	public void OpenMenu() => Shell.FlyoutIsPresented = true;

	public void CloseMenu() => Shell.FlyoutIsPresented = false;
}
