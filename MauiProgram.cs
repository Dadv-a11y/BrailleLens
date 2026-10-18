using BrailleLens.Core.Learning;
using BrailleLens.Pages;
using BrailleLens.Services;
using BrailleLens.ViewModels;
using Microsoft.Extensions.Logging;

namespace BrailleLens;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Inter-Regular.ttf", "Regular");
				fonts.AddFont("Inter-SemiBold.ttf", "Semibold");
				fonts.AddFont("Inter-Bold.ttf", "Bold");
			});

		builder.Services.AddSingleton(Preferences.Default);
		builder.Services.AddSingleton<IProgressStore, PreferencesProgressStore>();
		builder.Services.AddSingleton<ProgressService>();
		builder.Services.AddSingleton<SettingsService>();
		builder.Services.AddSingleton<NavigationService>();
		builder.Services.AddSingleton<DialogService>();
		builder.Services.AddSingleton<ScanService>();

		builder.Services.AddSingleton<ShellViewModel>();
		builder.Services.AddSingleton<AppShell>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<LearnViewModel>();
		builder.Services.AddTransient<LearnPage>();
		builder.Services.AddTransient<TrainingViewModel>();
		builder.Services.AddTransient<TrainingPage>();
		builder.Services.AddTransient<ConvertViewModel>();
		builder.Services.AddTransient<ConvertPage>();
		builder.Services.AddTransient<ScanViewModel>();
		builder.Services.AddTransient<ScanPage>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<HelpPage>();
		builder.Services.AddTransient<AboutPage>();

#if ANDROID
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, _) =>
			handler.PlatformView.Background = null);
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, _) =>
			handler.PlatformView.Background = null);
#endif

#if WINDOWS
		Microsoft.Maui.Controls.Handlers.ShellHandler.Mapper.AppendToMapping("UseInAppNavigation", (handler, _) =>
		{
			handler.PlatformView.IsPaneToggleButtonVisible = false;
			handler.PlatformView.IsBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed;
		});
		Microsoft.Maui.Handlers.SwitchHandler.Mapper.AppendToMapping("Compact", (handler, _) =>
		{
			handler.PlatformView.MinWidth = 0;
			handler.PlatformView.OnContent = null;
			handler.PlatformView.OffContent = null;
		});
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
