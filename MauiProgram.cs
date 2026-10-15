using BrailleLens.Pages;
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

		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<HomePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
