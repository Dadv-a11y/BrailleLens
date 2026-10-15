namespace BrailleLens.Controls;

public class SegmentedProgressBar : ContentView
{
	public static readonly BindableProperty TotalProperty =
		BindableProperty.Create(nameof(Total), typeof(int), typeof(SegmentedProgressBar), 10, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty ValueProperty =
		BindableProperty.Create(nameof(Value), typeof(int), typeof(SegmentedProgressBar), 0, propertyChanged: OnValueChanged);

	public static readonly BindableProperty SegmentHeightProperty =
		BindableProperty.Create(nameof(SegmentHeight), typeof(double), typeof(SegmentedProgressBar), 6.0, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty SpacingProperty =
		BindableProperty.Create(nameof(Spacing), typeof(double), typeof(SegmentedProgressBar), 4.0, propertyChanged: OnLayoutChanged);

	private readonly List<BoxView> _segments = new();

	public SegmentedProgressBar()
	{
		BuildLayout();
	}

	public int Total
	{
		get => (int)GetValue(TotalProperty);
		set => SetValue(TotalProperty, value);
	}

	public int Value
	{
		get => (int)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public double SegmentHeight
	{
		get => (double)GetValue(SegmentHeightProperty);
		set => SetValue(SegmentHeightProperty, value);
	}

	public double Spacing
	{
		get => (double)GetValue(SpacingProperty);
		set => SetValue(SpacingProperty, value);
	}

	private static void OnLayoutChanged(BindableObject bindable, object oldValue, object newValue) =>
		((SegmentedProgressBar)bindable).BuildLayout();

	private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue) =>
		((SegmentedProgressBar)bindable).UpdateSegments();

	private void BuildLayout()
	{
		var grid = new Grid { ColumnSpacing = Spacing, HeightRequest = SegmentHeight };
		_segments.Clear();
		for (var i = 0; i < Math.Max(1, Total); i++)
		{
			grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
			var segment = new BoxView { CornerRadius = SegmentHeight / 2, HeightRequest = SegmentHeight };
			AutomationProperties.SetIsInAccessibleTree(segment, false);
			_segments.Add(segment);
			grid.Add(segment, i);
		}
		Content = grid;
		UpdateSegments();
	}

	private void UpdateSegments()
	{
		var filled = Math.Clamp(Value, 0, Total);
		for (var i = 0; i < _segments.Count; i++)
		{
			_segments[i].Color = i < filled ? AppColors.Primary : AppColors.Border;
		}
		SemanticProperties.SetDescription(this, $"Progression : {filled} sur {Total}");
	}
}
