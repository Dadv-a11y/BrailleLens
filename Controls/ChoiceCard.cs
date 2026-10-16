using System.Windows.Input;
using BrailleLens.Core.Braille;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public enum ChoiceState
{
	Normal,
	Correct,
	Wrong
}

public class ChoiceCard : ContentView
{
	public static readonly BindableProperty LetterProperty =
		BindableProperty.Create(nameof(Letter), typeof(string), typeof(ChoiceCard), string.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty CaptionProperty =
		BindableProperty.Create(nameof(Caption), typeof(string), typeof(ChoiceCard), string.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty CellProperty =
		BindableProperty.Create(nameof(Cell), typeof(BrailleCellModel), typeof(ChoiceCard), BrailleCellModel.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty ShowCellProperty =
		BindableProperty.Create(nameof(ShowCell), typeof(bool), typeof(ChoiceCard), false, propertyChanged: OnStateChanged);

	public static readonly BindableProperty StateProperty =
		BindableProperty.Create(nameof(State), typeof(ChoiceState), typeof(ChoiceCard), ChoiceState.Normal, propertyChanged: OnStateChanged);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ChoiceCard));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ChoiceCard));

	private readonly Border _surface;
	private readonly Label _letter = new()
	{
		FontFamily = "Bold",
		FontSize = 26,
		VerticalTextAlignment = TextAlignment.Center,
	};
	private readonly Label _caption = new()
	{
		FontSize = 15,
		HorizontalTextAlignment = TextAlignment.Center,
	};
	private readonly BrailleCell _cell = new() { DotSize = 22 };
	private readonly Icon _stateIcon = new() { Size = 20, VerticalOptions = LayoutOptions.Center };
	private readonly HorizontalStackLayout _letterRow;
	private readonly VerticalStackLayout _cellColumn;
	private readonly Button _button;

	public ChoiceCard()
	{
		AutomationProperties.SetIsInAccessibleTree(_letter, false);
		AutomationProperties.SetIsInAccessibleTree(_caption, false);
		AutomationProperties.SetIsInAccessibleTree(_cell, false);

		_letterRow = new HorizontalStackLayout
		{
			Spacing = 8,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center,
			Children = { _letter, _stateIcon },
		};
		_cellColumn = new VerticalStackLayout
		{
			Spacing = 12,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center,
			Children = { _caption, _cell },
		};

		_surface = new Border
		{
			StrokeShape = new RoundRectangle { CornerRadius = 16 },
			Padding = new Thickness(12),
			InputTransparent = true,
			Content = new Grid { Children = { _letterRow, _cellColumn } },
		};
		AutomationProperties.SetIsInAccessibleTree(_surface, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 16,
			Text = string.Empty,
			Padding = 0,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.SetBinding(Button.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));

		Content = new Grid { Children = { _surface, _button } };
		UpdateAppearance();
	}

	public string Letter
	{
		get => (string)GetValue(LetterProperty);
		set => SetValue(LetterProperty, value);
	}

	public string Caption
	{
		get => (string)GetValue(CaptionProperty);
		set => SetValue(CaptionProperty, value);
	}

	public BrailleCellModel Cell
	{
		get => (BrailleCellModel)GetValue(CellProperty);
		set => SetValue(CellProperty, value);
	}

	public bool ShowCell
	{
		get => (bool)GetValue(ShowCellProperty);
		set => SetValue(ShowCellProperty, value);
	}

	public ChoiceState State
	{
		get => (ChoiceState)GetValue(StateProperty);
		set => SetValue(StateProperty, value);
	}

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public object? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue) =>
		((ChoiceCard)bindable).UpdateAppearance();

	private void UpdateAppearance()
	{
		var (background, stroke, thickness) = State switch
		{
			ChoiceState.Correct => (AppColors.SuccessSoft, AppColors.Success, 2.0),
			ChoiceState.Wrong => (AppColors.ErrorSoft, AppColors.Error, 2.0),
			_ => (AppColors.Surface, AppColors.Border, 1.0),
		};
		_surface.BackgroundColor = background;
		_surface.Stroke = new SolidColorBrush(stroke);
		_surface.StrokeThickness = thickness;

		_letterRow.IsVisible = !ShowCell;
		_cellColumn.IsVisible = ShowCell;
		_letter.Text = Letter;
		_letter.TextColor = AppColors.TextPrimary;
		_caption.Text = Caption;
		_caption.TextColor = AppColors.TextSecondary;
		_cell.Cell = Cell;

		_stateIcon.IsVisible = State != ChoiceState.Normal && !ShowCell;
		_stateIcon.Data = AppColors.Icon(State == ChoiceState.Correct ? "IconCheck" : "IconClose");
		_stateIcon.Color = State == ChoiceState.Correct ? AppColors.Success : AppColors.Error;

		var description = ShowCell ? $"{Caption}, {BrailleDescriber.DescribeDots(Cell)}" : Letter;
		description += State switch
		{
			ChoiceState.Correct => ", bonne réponse",
			ChoiceState.Wrong => ", mauvaise réponse",
			_ => string.Empty,
		};
		SemanticProperties.SetDescription(_button, description);
	}
}
