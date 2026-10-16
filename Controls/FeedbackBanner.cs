using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public class FeedbackBanner : ContentView
{
	public static readonly BindableProperty IsCorrectProperty =
		BindableProperty.Create(nameof(IsCorrect), typeof(bool), typeof(FeedbackBanner), true,
			propertyChanged: (bindable, _, _) => ((FeedbackBanner)bindable).UpdateAppearance());

	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(FeedbackBanner), string.Empty,
			propertyChanged: (bindable, _, _) => ((FeedbackBanner)bindable).UpdateAppearance());

	private readonly Border _surface;
	private readonly Icon _icon = new() { Size = 22, VerticalOptions = LayoutOptions.Center };
	private readonly Label _label = new()
	{
		FontFamily = "Semibold",
		FontSize = 16,
		VerticalTextAlignment = TextAlignment.Center,
	};

	public FeedbackBanner()
	{
		AutomationProperties.SetIsInAccessibleTree(_label, false);
		var row = new HorizontalStackLayout { Spacing = 10, Children = { _icon, _label } };
		_surface = new Border
		{
			StrokeThickness = 0,
			StrokeShape = new RoundRectangle { CornerRadius = 14 },
			Padding = new Thickness(16, 14),
			Content = row,
		};
		Content = _surface;
		UpdateAppearance();
	}

	public bool IsCorrect
	{
		get => (bool)GetValue(IsCorrectProperty);
		set => SetValue(IsCorrectProperty, value);
	}

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	private void UpdateAppearance()
	{
		_surface.BackgroundColor = IsCorrect ? AppColors.SuccessSoft : AppColors.ErrorSoft;
		_icon.Data = AppColors.Icon(IsCorrect ? "IconCheckCircle" : "IconErrorCircle");
		_icon.Color = IsCorrect ? AppColors.Success : AppColors.Error;
		_label.Text = Text;
		_label.TextColor = AppColors.TextPrimary;
		SemanticProperties.SetDescription(this, Text);
	}
}
