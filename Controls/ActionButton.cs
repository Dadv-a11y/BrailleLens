using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public enum ButtonVariant
{
	Primary,
	Secondary,
	Outline
}

public class ActionButton : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(ActionButton), string.Empty, propertyChanged: OnContentChanged);

	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(ActionButton), null, propertyChanged: OnContentChanged);

	public static readonly BindableProperty TrailingIconProperty =
		BindableProperty.Create(nameof(TrailingIcon), typeof(string), typeof(ActionButton), null, propertyChanged: OnContentChanged);

	public static readonly BindableProperty VariantProperty =
		BindableProperty.Create(nameof(Variant), typeof(ButtonVariant), typeof(ActionButton), ButtonVariant.Primary, propertyChanged: OnContentChanged);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ActionButton));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ActionButton));

	public static readonly BindableProperty HintProperty =
		BindableProperty.Create(nameof(Hint), typeof(string), typeof(ActionButton), null,
			propertyChanged: (bindable, _, value) => SemanticProperties.SetHint(((ActionButton)bindable)._button, (string?)value));

	private readonly Border _surface;
	private readonly Icon _leading = new() { Size = 20, VerticalOptions = LayoutOptions.Center };
	private readonly Icon _trailing = new() { Size = 20, VerticalOptions = LayoutOptions.Center };
	private readonly Label _label = new()
	{
		FontFamily = "Semibold",
		FontSize = 17,
		VerticalTextAlignment = TextAlignment.Center,
		LineBreakMode = LineBreakMode.TailTruncation,
	};
	private readonly Button _button;
	private bool _isPressed;

	public event EventHandler? Clicked;

	public ActionButton()
	{
		var row = new HorizontalStackLayout
		{
			Spacing = 10,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center,
			Children = { _leading, _label, _trailing },
		};

		_surface = new Border
		{
			StrokeShape = new RoundRectangle { CornerRadius = 14 },
			Padding = new Thickness(16, 0),
			Content = row,
			InputTransparent = true,
		};
		AutomationProperties.SetIsInAccessibleTree(_surface, false);
		AutomationProperties.SetIsInAccessibleTree(_label, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 14,
			Text = string.Empty,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.SetBinding(Button.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));
		_button.Pressed += (_, _) => SetPressed(true);
		_button.Released += (_, _) => SetPressed(false);
		_button.Clicked += (_, e) => Clicked?.Invoke(this, e);
		_button.PropertyChanged += (_, e) =>
		{
			if (e.PropertyName == nameof(IsEnabled))
			{
				UpdateAppearance();
			}
		};

		MinimumHeightRequest = 56;
		Content = new Grid { Children = { _surface, _button } };
		UpdateAppearance();
	}

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public string? Icon
	{
		get => (string?)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	public string? TrailingIcon
	{
		get => (string?)GetValue(TrailingIconProperty);
		set => SetValue(TrailingIconProperty, value);
	}

	public ButtonVariant Variant
	{
		get => (ButtonVariant)GetValue(VariantProperty);
		set => SetValue(VariantProperty, value);
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

	public string? Hint
	{
		get => (string?)GetValue(HintProperty);
		set => SetValue(HintProperty, value);
	}

	protected override void OnPropertyChanged(string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);
		if (propertyName == nameof(IsEnabled))
		{
			_button.IsEnabled = IsEnabled;
			UpdateAppearance();
		}
	}

	private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue) =>
		((ActionButton)bindable).UpdateAppearance();

	private void SetPressed(bool pressed)
	{
		_isPressed = pressed;
		UpdateAppearance();
	}

	private void UpdateAppearance()
	{
		var enabled = IsEnabled && _button.IsEnabled;
		var (background, pressed, foreground, stroke) = Variant switch
		{
			ButtonVariant.Secondary => (AppColors.PrimarySoft, AppColors.PrimarySoftPressed, AppColors.Primary, (Color?)null),
			ButtonVariant.Outline => (AppColors.Surface, AppColors.Background, AppColors.TextPrimary, AppColors.Border),
			_ => (AppColors.Primary, AppColors.PrimaryPressed, AppColors.OnPrimary, (Color?)null),
		};

		if (!enabled)
		{
			foreground = AppColors.TextDisabled;
			if (Variant == ButtonVariant.Primary)
			{
				background = AppColors.Border;
			}
		}

		_surface.BackgroundColor = enabled && _isPressed ? pressed : background;
		_surface.Stroke = stroke is null ? null : new SolidColorBrush(stroke);
		_surface.StrokeThickness = stroke is null ? 0 : 1;

		_label.Text = Text;
		_label.TextColor = foreground;
		_leading.Data = Icon;
		_leading.Color = foreground;
		_leading.IsVisible = !string.IsNullOrEmpty(Icon);
		_trailing.Data = TrailingIcon;
		_trailing.Color = foreground;
		_trailing.IsVisible = !string.IsNullOrEmpty(TrailingIcon);

		SemanticProperties.SetDescription(_button, Text);
	}
}
