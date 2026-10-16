using BrailleLens.Controls;
using BrailleLens.ViewModels;

namespace BrailleLens.Sheets;

public partial class ExercisePickerView : ContentView
{
	public ExercisePickerView()
	{
		InitializeComponent();
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();
		OptionList.Clear();
		if (BindingContext is not TrainingViewModel viewModel)
		{
			return;
		}

		foreach (var option in viewModel.Exercises)
		{
			var row = new OptionRow
			{
				BindingContext = option,
				Title = option.Title,
				Description = option.Description,
				Icon = AppColors.Icon(option.IconKey),
				Command = viewModel.SelectExerciseCommand,
				CommandParameter = option.Type,
			};
			row.SetBinding(OptionRow.IsSelectedProperty, new Binding(nameof(ExerciseOption.IsSelected)));
			OptionList.Add(row);
		}
	}
}
