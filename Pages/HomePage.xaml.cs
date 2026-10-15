using BrailleLens.ViewModels;

namespace BrailleLens.Pages;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
