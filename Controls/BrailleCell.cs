using System.Windows.Input;
using BrailleLens.Core.Braille;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public class BrailleCell : ContentView
{
	private const double MinimumTouchSize = 48;

	public static readonly BindableProperty CellProperty =
		BindableProperty.Create(nameof(Cell), typeof(BrailleCellModel), typeof(BrailleCell), BrailleCellModel.Empty,
			BindingMode.TwoWay, propertyChanged: OnCellChanged);

	public static readonly BindableProperty IsEditableProperty =
		BindableProperty.Create(nameof(IsEditable), typeof(bool), typeof(BrailleCell), false, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty IsExplorableProperty =
		BindableProperty.Create(nameof(IsExplorable), typeof(bool), typeof(BrailleCell), false, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty DotCommandProperty =
		BindableProperty.Create(nameof(DotCommand), typeof(ICommand), typeof(BrailleCell));

	public static readonly BindableProperty ShowNumbersProperty =
		BindableProperty.Create(nameof(ShowNumbers), typeof(bool), typeof(BrailleCell), false, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty DotSizeProperty =
		BindableProperty.Create(nameof(DotSize), typeof(double), typeof(BrailleCell), 44.0, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty RaisedColorProperty =
		BindableProperty.Create(nameof(RaisedColor), typeof(Color), typeof(BrailleCell), null, propertyChanged: OnCellChanged);

	private readonly Ellipse[] _dots = new Ellipse[6];
	private readonly Button?[] _dotButtons = new Button?[6];

	public event EventHandler<BrailleCellModel>? CellChanged;

	public event EventHandler<int>? DotTapped;

	public BrailleCell()
	{
		BuildLayout();
	}

	public BrailleCellModel Cell
	{
		get => (BrailleCellModel)GetValue(CellProperty);
		set => SetValue(CellProperty, value);
	}

	public bool IsEditable
	{
		get => (bool)GetValue(IsEditableProperty);
		set => SetValue(IsEditableProperty, value);
	}

	public bool IsExplorable
	{
		get => (bool)GetValue(IsExplorableProperty);
		set => SetValue(IsExplorableProperty, value);
	}

	public ICommand? DotCommand
	{
		get => (ICommand?)GetValue(DotCommandProperty);
		set => SetValue(DotCommandProperty, value);
	}

	private bool IsInteractive => IsEditable || IsExplorable;

	public bool ShowNumbers
	{
		get => (bool)GetValue(ShowNumbersProperty);
		set => SetValue(ShowNumbersProperty, value);
	}

	public double DotSize
	{
		get => (double)GetValue(DotSizeProperty);
		set => SetValue(DotSizeProperty, value);
	}

	public Color? RaisedColor
	{
		get => (Color?)GetValue(RaisedColorProperty);
		set => SetValue(RaisedColorProperty, value);
	}

	private static void OnCellChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (BrailleCell)bindable;
		control.UpdateDots();
		if (oldValue is BrailleCellModel previous && newValue is BrailleCellModel current && previous != current)
		{
			control.CellChanged?.Invoke(control, current);
		}
	}

	private static void OnLayoutChanged(BindableObject bindable, object oldValue, object newValue) =>
		((BrailleCell)bindable).BuildLayout();

	private void BuildLayout()
	{
		var slotSize = IsInteractive ? Math.Max(DotSize, MinimumTouchSize) : DotSize;
		var numberSize = ShowNumbers ? Math.Max(18, DotSize * 0.4) : 0;

		var grid = new Grid
		{
			ColumnSpacing = DotSize * (ShowNumbers ? 0.4 : 0.35),
			RowSpacing = DotSize * (ShowNumbers ? 0.2 : 0.3),
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center,
		};
		for (var i = 0; i < 2; i++)
		{
			grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
		}
		for (var i = 0; i < 3; i++)
		{
			grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
		}

		for (var index = 0; index < 6; index++)
		{
			var dotNumber = index + 1;
			var slot = new Grid
			{
				ColumnDefinitions = { new ColumnDefinition(numberSize), new ColumnDefinition(slotSize) },
				RowDefinitions = { new RowDefinition(numberSize), new RowDefinition(slotSize) },
			};

			if (ShowNumbers)
			{
				var number = new Label
				{
					Text = dotNumber.ToString(),
					FontSize = Math.Max(11, DotSize * 0.26),
					TextColor = AppColors.TextSecondary,
					HorizontalTextAlignment = TextAlignment.Start,
					VerticalTextAlignment = TextAlignment.End,
				};
				AutomationProperties.SetIsInAccessibleTree(number, false);
				slot.Add(number, 0, 0);
			}

			var dot = new Ellipse
			{
				WidthRequest = DotSize,
				HeightRequest = DotSize,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				StrokeThickness = Math.Max(1.5, DotSize / 22),
				InputTransparent = true,
			};
			AutomationProperties.SetIsInAccessibleTree(dot, false);
			_dots[index] = dot;
			slot.Add(dot, 1, 1);

			_dotButtons[index] = null;
			if (IsInteractive)
			{
				var button = new Button
				{
					Style = new Style(typeof(Button)),
					BackgroundColor = Colors.Transparent,
					BorderWidth = 0,
					CornerRadius = (int)(slotSize / 2),
					Text = string.Empty,
					WidthRequest = slotSize,
					HeightRequest = slotSize,
					MinimumHeightRequest = 0,
					MinimumWidthRequest = 0,
					Padding = 0,
				};
				button.Clicked += (_, _) => OnDotClicked(dotNumber);
				_dotButtons[index] = button;
				slot.Add(button, 1, 1);
			}

			grid.Add(slot, index < 3 ? 0 : 1, index % 3);
		}

		AutomationProperties.SetIsInAccessibleTree(this, !IsInteractive);
		Content = grid;
		UpdateDots();
	}

	private void OnDotClicked(int dot)
	{
		if (IsEditable)
		{
			Cell = Cell.Toggle(dot);
			SemanticScreenReader.Announce(DescribeDot(dot));
		}

		DotTapped?.Invoke(this, dot);
		if (DotCommand?.CanExecute(dot) == true)
		{
			DotCommand.Execute(dot);
		}
	}

	private string DescribeDot(int dot) => IsEditable
		? $"Point {dot}, {(Cell.IsRaised(dot) ? "activé" : "désactivé")}"
		: $"Point {dot}, {(Cell.IsRaised(dot) ? "levé" : "non levé")}";

	private void UpdateDots()
	{
		var raisedColor = RaisedColor ?? AppColors.Primary;
		for (var index = 0; index < 6; index++)
		{
			var dot = _dots[index];
			if (dot is null)
			{
				continue;
			}

			var raised = Cell.IsRaised(index + 1);
			dot.Fill = raised ? new SolidColorBrush(raisedColor) : Brush.Transparent;
			dot.Stroke = new SolidColorBrush(raised ? raisedColor : AppColors.DotInactive);

			if (_dotButtons[index] is { } button)
			{
				SemanticProperties.SetDescription(button, DescribeDot(index + 1));
			}
		}

		SemanticProperties.SetDescription(this, $"Cellule braille, {BrailleDescriber.DescribeDots(Cell)}");
	}
}
