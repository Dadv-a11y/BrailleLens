using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public class LetterTile : ContentView
{
	public static readonly BindableProperty LetterProperty =
		BindableProperty.Create(nameof(Letter), typeof(string), typeof(LetterTile), string.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty IsMasteredProperty =
		BindableProperty.Create(nameof(IsMastered), typeof(bool), typeof(LetterTile), false, propertyChanged: OnStateChanged);

	public static readonly BindableProperty IsSelectedProperty =
		BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(LetterTile), false, propertyChanged: OnStateChanged);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LetterTile));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LetterTile));

	private readonly Border _surface;
	private readonly Label _label = new()
	{
		FontFamily = "Semibold",
		FontSize = 19,
		HorizontalTextAlignment = TextAlignment.Center,
	};
	private readonly Icon _check = new() { Size = 16, HorizontalOptions = LayoutOptions.Center };
	private readonly Button _button;

	public LetterTile()
	{
		_check.Data = AppColors.Icon("IconCheck");
		_check.Color = AppColors.Primary;
		AutomationProperties.SetIsInAccessibleTree(_label, false);

		_surface = new Border
		{
			StrokeShape = new RoundRectangle { CornerRadius = 12 },
			InputTransparent = true,
			Content = new VerticalStackLayout
			{
				Spacing = 2,
				VerticalOptions = LayoutOptions.Center,
				Children = { _label, _check },
			},
		};
		AutomationProperties.SetIsInAccessibleTree(_surface, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 12,
			Text = string.Empty,
			Padding = 0,
			MinimumWidthRequest = 0,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.SetBinding(Button.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));

		MinimumHeightRequest = 64;
		Content = new Grid { Children = { _surface, _button } };
		UpdateAppearance();
	}

	public string Letter
	{
		get => (string)GetValue(LetterProperty);
		set => SetValue(LetterProperty, value);
	}

	public bool IsMastered
	{
		get => (bool)GetValue(IsMasteredProperty);
		set => SetValue(IsMasteredProperty, value);
	}

	public bool IsSelected
	{
		get => (bool)GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
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
		((LetterTile)bindable).UpdateAppearance();

	private void UpdateAppearance()
	{
		_label.Text = Letter;
		_label.TextColor = AppColors.TextPrimary;
		_check.IsVisible = IsMastered;
		_surface.BackgroundColor = IsMastered ? AppColors.PrimarySoft : AppColors.Surface;
		_surface.Stroke = new SolidColorBrush(IsSelected ? AppColors.Primary : AppColors.Border);
		_surface.StrokeThickness = IsSelected ? 2 : 1;

		var states = new List<string> { Letter };
		if (IsMastered)
		{
			states.Add("maîtrisée");
		}
		if (IsSelected)
		{
			states.Add("sélectionnée");
		}
		SemanticProperties.SetDescription(_button, string.Join(", ", states));
	}
}
