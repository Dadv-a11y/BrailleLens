using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

[ContentProperty(nameof(SheetContent))]
public class BottomSheet : ContentView
{
	private const double MaxHeightRatio = 0.9;

	public static readonly BindableProperty IsOpenProperty =
		BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(BottomSheet), false, BindingMode.TwoWay,
			propertyChanged: (bindable, _, value) => ((BottomSheet)bindable).OnIsOpenChanged((bool)value));

	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomSheet), string.Empty,
			propertyChanged: (bindable, _, value) => ((BottomSheet)bindable)._title.Text = (string)value);

	public static readonly BindableProperty SheetContentProperty =
		BindableProperty.Create(nameof(SheetContent), typeof(View), typeof(BottomSheet), null,
			propertyChanged: (bindable, _, value) => ((BottomSheet)bindable)._body.Content = (View?)value);

	private readonly Label _title;
	private readonly ScrollView _body = new();
	private readonly Border _sheet;

	public event EventHandler? Closed;

	public BottomSheet()
	{
		var scrim = new BoxView { Color = AppColors.Overlay };
		var scrimTap = new TapGestureRecognizer();
		scrimTap.Tapped += (_, _) => Close();
		scrim.GestureRecognizers.Add(scrimTap);
		AutomationProperties.SetIsInAccessibleTree(scrim, false);

		var handle = new BoxView
		{
			Color = AppColors.DotInactive,
			WidthRequest = 40,
			HeightRequest = 4,
			CornerRadius = 2,
			HorizontalOptions = LayoutOptions.Center,
			Margin = new Thickness(0, 0, 0, 12),
		};
		AutomationProperties.SetIsInAccessibleTree(handle, false);

		_title = new Label
		{
			FontFamily = "Semibold",
			FontAttributes = FontAttributes.Bold,
			FontSize = 24,
			VerticalTextAlignment = TextAlignment.Center,
		};
		SemanticProperties.SetHeadingLevel(_title, SemanticHeadingLevel.Level2);

		var close = new IconButton
		{
			Icon = AppColors.Icon("IconClose"),
			Description = "Fermer",
			Margin = new Thickness(0, 0, -12, 0),
		};
		close.Clicked += (_, _) => Close();

		var header = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Margin = new Thickness(0, 0, 0, 12),
		};
		header.Add(_title, 0);
		header.Add(close, 1);

		var layout = new Grid
		{
			RowDefinitions =
			{
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Star),
			},
		};
		layout.Add(handle, 0, 0);
		layout.Add(header, 0, 1);
		layout.Add(_body, 0, 2);

		_sheet = new Border
		{
			BackgroundColor = AppColors.Surface,
			StrokeThickness = 0,
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(24, 24, 0, 0) },
			Padding = new Thickness(24, 12, 24, 24),
			VerticalOptions = LayoutOptions.End,
			Content = layout,
		};

		IsVisible = false;
		Content = new Grid { Children = { scrim, _sheet } };
	}

	public bool IsOpen
	{
		get => (bool)GetValue(IsOpenProperty);
		set => SetValue(IsOpenProperty, value);
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public View? SheetContent
	{
		get => (View?)GetValue(SheetContentProperty);
		set => SetValue(SheetContentProperty, value);
	}

	public void Open() => IsOpen = true;

	public void Close() => IsOpen = false;

	public bool TryClose()
	{
		if (!IsOpen)
		{
			return false;
		}
		Close();
		return true;
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		if (height > 0)
		{
			_sheet.MaximumHeightRequest = height * MaxHeightRatio;
		}
	}

	private void OnIsOpenChanged(bool isOpen)
	{
		IsVisible = isOpen;
		if (isOpen)
		{
			if (_title.Handler is not null)
			{
				Dispatcher.Dispatch(() => _title.SetSemanticFocus());
			}
		}
		else
		{
			Closed?.Invoke(this, EventArgs.Empty);
		}
	}
}
