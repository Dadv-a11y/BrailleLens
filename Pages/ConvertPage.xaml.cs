using BrailleLens.ViewModels;

namespace BrailleLens.Pages;

public partial class ConvertPage : ContentPage
{
	private readonly ConvertViewModel _viewModel;

	public ConvertPage(ConvertViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.Load();
	}
}
