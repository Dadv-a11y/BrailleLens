using System.Windows.Input;

namespace BrailleLens.Controls;

public class IconButton : ContentView
{
	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(IconButton), null,
			propertyChanged: (bindable, _, value) => ((IconButton)bindable)._icon.Data = (string?)value);

	public static readonly BindableProperty ColorProperty =
		BindableProperty.Create(nameof(Color), typeof(Color), typeof(IconButton), null,
			propertyChanged: (bindable, _, value) => ((IconButton)bindable)._icon.Color = (Color?)value);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(IconButton), null,
			propertyChanged: (bindable, _, value) => SemanticProperties.SetDescription(((IconButton)bindable)._button, (string?)value));

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(IconButton));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(IconButton));

	private readonly Icon _icon = new() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
	private readonly Button _button;

	public event EventHandler? Clicked;

	public IconButton()
	{
		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 24,
			Text = string.Empty,
			Padding = 0,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.SetBinding(Button.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));
		_button.Clicked += (_, e) => Clicked?.Invoke(this, e);

		WidthRequest = 48;
		HeightRequest = 48;
		Content = new Grid { Children = { _icon, _button } };
	}

	public string? Icon
	{
		get => (string?)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	public Color? Color
	{
		get => (Color?)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

	public string? Description
	{
		get => (string?)GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
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
}
