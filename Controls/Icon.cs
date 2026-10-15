using Microsoft.Maui.Controls.Shapes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace BrailleLens.Controls;

public class Icon : ContentView
{
	private const double GridSize = 24;

	private static readonly PathGeometryConverter GeometryConverter = new();

	public static readonly BindableProperty DataProperty =
		BindableProperty.Create(nameof(Data), typeof(string), typeof(Icon), null, propertyChanged: OnDataChanged);

	public static readonly BindableProperty ColorProperty =
		BindableProperty.Create(nameof(Color), typeof(Color), typeof(Icon), null, propertyChanged: OnAppearanceChanged);

	public static readonly BindableProperty SizeProperty =
		BindableProperty.Create(nameof(Size), typeof(double), typeof(Icon), 24.0, propertyChanged: OnAppearanceChanged);

	public static readonly BindableProperty StrokeThicknessProperty =
		BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(Icon), 2.0, propertyChanged: OnAppearanceChanged);

	private readonly Path _path = new()
	{
		Aspect = Stretch.None,
		StrokeLineCap = PenLineCap.Round,
		StrokeLineJoin = PenLineJoin.Round,
		HorizontalOptions = LayoutOptions.Start,
		VerticalOptions = LayoutOptions.Start,
	};

	public Icon()
	{
		InputTransparent = true;
		AutomationProperties.SetIsInAccessibleTree(this, false);
		Content = _path;
		UpdateAppearance();
	}

	public string? Data
	{
		get => (string?)GetValue(DataProperty);
		set => SetValue(DataProperty, value);
	}

	public Color? Color
	{
		get => (Color?)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

	public double Size
	{
		get => (double)GetValue(SizeProperty);
		set => SetValue(SizeProperty, value);
	}

	public double StrokeThickness
	{
		get => (double)GetValue(StrokeThicknessProperty);
		set => SetValue(StrokeThicknessProperty, value);
	}

	private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var icon = (Icon)bindable;
		icon._path.Data = string.IsNullOrWhiteSpace(newValue as string)
			? null
			: (Geometry?)GeometryConverter.ConvertFromInvariantString((string)newValue);
	}

	private static void OnAppearanceChanged(BindableObject bindable, object oldValue, object newValue) =>
		((Icon)bindable).UpdateAppearance();

	private void UpdateAppearance()
	{
		var scale = Size / GridSize;
		WidthRequest = Size;
		HeightRequest = Size;
		_path.WidthRequest = Size;
		_path.HeightRequest = Size;
		_path.RenderTransform = new ScaleTransform(scale, scale);
		_path.StrokeThickness = StrokeThickness;
		_path.Stroke = new SolidColorBrush(Color ?? AppColors.TextPrimary);
	}
}
