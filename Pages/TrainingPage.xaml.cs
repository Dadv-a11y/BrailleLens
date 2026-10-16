using System.ComponentModel;
using BrailleLens.ViewModels;

namespace BrailleLens.Pages;

public partial class TrainingPage : ContentPage
{
	private readonly TrainingViewModel _viewModel;

	public TrainingPage(TrainingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
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

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(TrainingViewModel.IsAnswered))
		{
			Dispatcher.Dispatch(async () =>
			{
				await Task.Delay(100);
				if (_viewModel.IsAnswered)
				{
					await Scroller.ScrollToAsync(Feedback, ScrollToPosition.End, false);
				}
				else
				{
					await Scroller.ScrollToAsync(0, 0, false);
				}
			});
		}
		else if (e.PropertyName == nameof(TrainingViewModel.IsFinished) && _viewModel.IsFinished)
		{
			Dispatcher.Dispatch(async () => await Scroller.ScrollToAsync(0, 0, false));
		}
	}
}
