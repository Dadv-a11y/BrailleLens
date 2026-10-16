using BrailleLens.Controls;
using BrailleLens.ViewModels;

namespace BrailleLens.Sheets;

public partial class LetterPickerView : ContentView
{
	private const int Columns = 6;

	public LetterPickerView()
	{
		InitializeComponent();
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();
		BuildTiles();
	}

	private void BuildTiles()
	{
		LetterGrid.Clear();
		LetterGrid.RowDefinitions.Clear();
		if (BindingContext is not LearnViewModel viewModel)
		{
			return;
		}

		for (var index = 0; index < viewModel.Letters.Count; index++)
		{
			if (index % Columns == 0)
			{
				LetterGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
			}

			var item = viewModel.Letters[index];
			var tile = new LetterTile
			{
				BindingContext = item,
				Letter = item.Text,
				Command = viewModel.SelectLetterCommand,
				CommandParameter = item.Letter,
			};
			tile.SetBinding(LetterTile.IsMasteredProperty, new Binding(nameof(LetterItem.IsMastered)));
			tile.SetBinding(LetterTile.IsSelectedProperty, new Binding(nameof(LetterItem.IsSelected)));
			LetterGrid.Add(tile, index % Columns, index / Columns);
		}
	}
}
