using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace BrailleLens.Controls;

public class OptionRow : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(OptionRow), string.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(OptionRow), string.Empty, propertyChanged: OnStateChanged);

	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(OptionRow), null, propertyChanged: OnStateChanged);

	public static readonly BindableProperty IsSelectedProperty =
		BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(OptionRow), false, propertyChanged: OnStateChanged);

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(OptionRow));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(OptionRow));

	private readonly Border _surface;
	private readonly Icon _icon = new() { Size = 26, VerticalOptions = LayoutOptions.Center };
	private readonly Icon _check = new() { Size = 22, VerticalOptions = LayoutOptions.Center };
	private readonly Label _title = new() { FontFamily = "Semibold", FontSize = 17 };
	private readonly Label _description = new() { FontSize = 15 };
	private readonly Button _button;

	public OptionRow()
	{
		AutomationProperties.SetIsInAccessibleTree(_title, false);
		AutomationProperties.SetIsInAccessibleTree(_description, false);
		_check.Data = AppColors.Icon("IconCheck");

		var grid = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Auto),
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto),
			},
			ColumnSpacing = 16,
		};
		grid.Add(_icon, 0);
		grid.Add(new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center, Children = { _title, _description } }, 1);
		grid.Add(_check, 2);

		_surface = new Border
		{
			StrokeShape = new RoundRectangle { CornerRadius = 14 },
			Padding = new Thickness(18, 14),
			InputTransparent = true,
			Content = grid,
		};
		AutomationProperties.SetIsInAccessibleTree(_surface, false);

		_button = new Button
		{
			Style = new Style(typeof(Button)),
			BackgroundColor = Colors.Transparent,
			BorderWidth = 0,
			CornerRadius = 14,
			Text = string.Empty,
			Padding = 0,
		};
		_button.SetBinding(Button.CommandProperty, new Binding(nameof(Command), source: this));
		_button.SetBinding(Button.CommandParameterProperty, new Binding(nameof(CommandParameter), source: this));

		Content = new Grid { Children = { _surface, _button } };
		UpdateAppearance();
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string Description
	{
		get => (string)GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	public string? Icon
	{
		get => (string?)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
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
		((OptionRow)bindable).UpdateAppearance();

	private void UpdateAppearance()
	{
		_surface.BackgroundColor = IsSelected ? AppColors.PrimarySoft : AppColors.Surface;
		_surface.Stroke = new SolidColorBrush(IsSelected ? AppColors.Primary : AppColors.Border);
		_surface.StrokeThickness = 1;
		_icon.Data = Icon;
		_icon.Color = AppColors.Primary;
		_check.Color = AppColors.Primary;
		_check.IsVisible = IsSelected;
		_title.Text = Title;
		_title.TextColor = AppColors.TextPrimary;
		_description.Text = Description;
		_description.TextColor = AppColors.TextSecondary;

		var state = IsSelected ? ", sélectionné" : string.Empty;
		SemanticProperties.SetDescription(_button, $"{Title}{state}. {Description}");
	}
}
