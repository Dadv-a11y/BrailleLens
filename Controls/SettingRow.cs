namespace BrailleLens.Controls;

public class SettingRow : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(SettingRow), string.Empty, propertyChanged: OnTextChanged);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(SettingRow), string.Empty, propertyChanged: OnTextChanged);

	public static readonly BindableProperty IsOnProperty =
		BindableProperty.Create(nameof(IsOn), typeof(bool), typeof(SettingRow), false, BindingMode.TwoWay,
			propertyChanged: (bindable, _, value) => ((SettingRow)bindable)._switch.IsToggled = (bool)value);

	private readonly Label _title = new() { FontFamily = "Semibold", FontSize = 17 };
	private readonly Label _description = new() { FontSize = 15 };
	private readonly Switch _switch = new() { VerticalOptions = LayoutOptions.Center };

	public SettingRow()
	{
		_title.TextColor = AppColors.TextPrimary;
		_description.TextColor = AppColors.TextSecondary;
		_switch.OnColor = AppColors.Primary;
		_switch.ThumbColor = AppColors.Surface;
		AutomationProperties.SetIsInAccessibleTree(_title, false);
		AutomationProperties.SetIsInAccessibleTree(_description, false);
		_switch.Toggled += (_, e) => IsOn = e.Value;

		var grid = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			ColumnSpacing = 16,
			MinimumHeightRequest = 56,
		};
		grid.Add(new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center, Children = { _title, _description } }, 0);
		grid.Add(_switch, 1);
		Content = grid;
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

	public bool IsOn
	{
		get => (bool)GetValue(IsOnProperty);
		set => SetValue(IsOnProperty, value);
	}

	private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var row = (SettingRow)bindable;
		row._title.Text = row.Title;
		row._description.Text = row.Description;
		SemanticProperties.SetDescription(row._switch, row.Title);
		SemanticProperties.SetHint(row._switch, row.Description);
	}
}
