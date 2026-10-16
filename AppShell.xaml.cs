using BrailleLens.Pages;
using BrailleLens.Services;
using BrailleLens.ViewModels;

namespace BrailleLens;

public partial class AppShell : Shell
{
	public AppShell(ShellViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

		Routing.RegisterRoute(Routes.Learn, typeof(LearnPage));
		Routing.RegisterRoute(Routes.Training, typeof(TrainingPage));
		Routing.RegisterRoute(Routes.Convert, typeof(ConvertPage));
		Routing.RegisterRoute(Routes.Scan, typeof(ScanPage));
		Routing.RegisterRoute(Routes.Settings, typeof(SettingsPage));
		Routing.RegisterRoute(Routes.Help, typeof(HelpPage));
		Routing.RegisterRoute(Routes.About, typeof(AboutPage));
	}
}
