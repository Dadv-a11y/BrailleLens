using BrailleLens.ViewModels;

namespace BrailleLens.Pages;

public partial class LearnPage : ContentPage
{
	private readonly LearnViewModel _viewModel;

	public LearnPage(LearnViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.Load();
	}

	protected override bool OnBackButtonPressed()
	{
		if (_viewModel.IsPickerOpen)
		{
			_viewModel.IsPickerOpen = false;
			return true;
		}
		return base.OnBackButtonPressed();
	}
}
