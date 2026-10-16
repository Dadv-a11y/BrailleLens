using System.Windows.Input;

namespace BrailleLens.Controls;

[ContentProperty(nameof(TrailingContent))]
public class PageHeader : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(PageHeader), string.Empty,
			propertyChanged: (bindable, _, value) => ((PageHeader)bindable)._title.Text = (string)value);

	public static readonly BindableProperty TrailingContentProperty =
		BindableProperty.Create(nameof(TrailingContent), typeof(View), typeof(PageHeader), null,
			propertyChanged: (bindable, _, value) => ((PageHeader)bindable)._trailing.Content = (View?)value);

	public static readonly BindableProperty BackCommandProperty =
		BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(PageHeader));

	private readonly Label _title = new()
	{
		FontFamily = "Bold",
		FontSize = 26,
		VerticalTextAlignment = TextAlignment.Center,
		LineBreakMode = LineBreakMode.TailTruncation,
	};
	private readonly ContentView _trailing = new() { VerticalOptions = LayoutOptions.Center };

	public PageHeader()
	{
		_title.TextColor = AppColors.TextPrimary;
		SemanticProperties.SetHeadingLevel(_title, SemanticHeadingLevel.Level1);

		var back = new IconButton { Icon = AppColors.Icon("IconArrowLeft"), Description = "Retour" };
		back.Clicked += OnBackClicked;

		var grid = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Auto),
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto),
			},
			ColumnSpacing = 8,
			Padding = new Thickness(12, 16, 24, 8),
		};
		grid.Add(back, 0);
		grid.Add(_title, 1);
		grid.Add(_trailing, 2);
		Content = grid;
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public View? TrailingContent
	{
		get => (View?)GetValue(TrailingContentProperty);
		set => SetValue(TrailingContentProperty, value);
	}

	public ICommand? BackCommand
	{
		get => (ICommand?)GetValue(BackCommandProperty);
		set => SetValue(BackCommandProperty, value);
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		if (BackCommand is { } command)
		{
			if (command.CanExecute(null))
			{
				command.Execute(null);
			}
			return;
		}

		if (Shell.Current is { } shell)
		{
			await shell.GoToAsync("..");
		}
	}
}
