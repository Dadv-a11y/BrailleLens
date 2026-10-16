using System.Windows.Input;

namespace BrailleLens.Controls;

public class MenuRow : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(MenuRow), string.Empty,
			propertyChanged: (bindable, _, value) => ((MenuRow)bindable).UpdateText((string)value));

	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(MenuRow), null,
			propertyChanged: (bindable, _, value) => ((MenuRow)bindable)._icon.Data = (string?)value);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MenuRow));

	private readonly Icon _icon = new() { VerticalOptions = LayoutOptions.Center };
	private readonly Label _label = new() { FontSize = 17, VerticalTextAlignment = TextAlignment.Center };
	private readonly Button _button;

	public MenuRow()
	{
		var row = new HorizontalStackLayout
		{
			Spacing = 16,
			VerticalOptions = LayoutOptions.Center,
			InputTransparent = true,
			Children = { _icon, _label },
		};
		AutomationProperties.SetIsInAccessibleTree(row, false);
		AutomationProperties.SetIsInAccessibleTree(_label, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 12,
			Text = string.Empty,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));

		HeightRequest = 56;
		Content = new Grid { Children = { row, _button } };
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

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	private void UpdateText(string text)
	{
		_label.Text = text;
		SemanticProperties.SetDescription(_button, text);
	}
}
