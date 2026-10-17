using BrailleLens.ViewModels;

namespace BrailleLens.Pages;

public partial class ScanPage : ContentPage
{
	public ScanPage(ScanViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
