using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public class SelectorField : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(SelectorField), string.Empty,
			propertyChanged: (bindable, _, _) => ((SelectorField)bindable).UpdateText());

	public static readonly BindableProperty AccessibleLabelProperty =
		BindableProperty.Create(nameof(AccessibleLabel), typeof(string), typeof(SelectorField), null,
			propertyChanged: (bindable, _, _) => ((SelectorField)bindable).UpdateText());

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SelectorField));

	private readonly Label _label = new()
	{
		FontFamily = "Semibold",
		FontSize = 17,
		VerticalTextAlignment = TextAlignment.Center,
		LineBreakMode = LineBreakMode.TailTruncation,
	};
	private readonly Button _button;

	public event EventHandler? Clicked;

	public SelectorField()
	{
		_label.TextColor = AppColors.Primary;
		var chevron = new Icon { Data = AppColors.Icon("IconChevronDown"), Color = AppColors.Primary, Size = 22 };

		var row = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			ColumnSpacing = 12,
		};
		row.Add(_label, 0);
		row.Add(chevron, 1);

		var surface = new Border
		{
			BackgroundColor = AppColors.PrimarySoft,
			StrokeThickness = 0,
			StrokeShape = new RoundRectangle { CornerRadius = 14 },
			Padding = new Thickness(18, 0),
			Content = row,
			InputTransparent = true,
		};
		AutomationProperties.SetIsInAccessibleTree(surface, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 14,
			Text = string.Empty,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.Clicked += (_, e) => Clicked?.Invoke(this, e);
		SemanticProperties.SetHint(_button, "Ouvre la liste des choix");

		HeightRequest = 56;
		Content = new Grid { Children = { surface, _button } };
	}

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public string? AccessibleLabel
	{
		get => (string?)GetValue(AccessibleLabelProperty);
		set => SetValue(AccessibleLabelProperty, value);
	}

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	private void UpdateText()
	{
		_label.Text = Text;
		SemanticProperties.SetDescription(_button, string.IsNullOrEmpty(AccessibleLabel) ? Text : $"{AccessibleLabel} : {Text}");
	}
}
