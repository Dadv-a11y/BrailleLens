using BrailleLens.Core.Braille;

namespace BrailleLens.Controls;

public class BrailleStrip : ContentView
{
	public static readonly BindableProperty TranslationProperty =
		BindableProperty.Create(nameof(Translation), typeof(BrailleTranslation), typeof(BrailleStrip), null, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(BrailleStrip), null,
			propertyChanged: (bindable, _, value) => ((BrailleStrip)bindable).Translation = BrailleTranslator.ToBraille((string?)value));

	public static readonly BindableProperty ShowLettersProperty =
		BindableProperty.Create(nameof(ShowLetters), typeof(bool), typeof(BrailleStrip), true, propertyChanged: OnLayoutChanged);

	public static readonly BindableProperty DotSizeProperty =
		BindableProperty.Create(nameof(DotSize), typeof(double), typeof(BrailleStrip), 12.0, propertyChanged: OnLayoutChanged);

	private readonly VerticalStackLayout _rows = new();
	private int _itemsPerRow = int.MaxValue;

	public BrailleStrip()
	{
		Content = _rows;
	}

	public BrailleTranslation? Translation
	{
		get => (BrailleTranslation?)GetValue(TranslationProperty);
		set => SetValue(TranslationProperty, value);
	}

	public string? Text
	{
		get => (string?)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public bool ShowLetters
	{
		get => (bool)GetValue(ShowLettersProperty);
		set => SetValue(ShowLettersProperty, value);
	}

	public double DotSize
	{
		get => (double)GetValue(DotSizeProperty);
		set => SetValue(DotSizeProperty, value);
	}

	private double ItemSpacing => DotSize * 1.2;

	private double ItemWidth => DotSize * 2.35;

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		if (width <= 0)
		{
			return;
		}

		var perRow = Math.Max(1, (int)((width + ItemSpacing) / (ItemWidth + ItemSpacing)));
		if (perRow != _itemsPerRow)
		{
			_itemsPerRow = perRow;
			BuildLayout();
		}
	}

	private static void OnLayoutChanged(BindableObject bindable, object oldValue, object newValue) =>
		((BrailleStrip)bindable).BuildLayout();

	private void BuildLayout()
	{
		_rows.Clear();
		_rows.Spacing = DotSize;
		var tokens = Translation?.Tokens ?? Array.Empty<BrailleToken>();

		HorizontalStackLayout? row = null;
		for (var index = 0; index < tokens.Count; index++)
		{
			if (index % _itemsPerRow == 0)
			{
				row = new HorizontalStackLayout { Spacing = ItemSpacing };
				_rows.Add(row);
			}
			row!.Add(CreateItem(tokens[index]));
		}
	}

	private View CreateItem(BrailleToken token)
	{
		var item = new VerticalStackLayout { Spacing = DotSize * 0.6, WidthRequest = ItemWidth };
		if (token.HasCell)
		{
			var cell = new BrailleCell { Cell = token.Cell, DotSize = DotSize, HorizontalOptions = LayoutOptions.Center };
			AutomationProperties.SetIsInAccessibleTree(cell, false);
			item.Add(cell);
		}
		else
		{
			item.Add(new Label
			{
				Text = token.Source.ToString(),
				TextColor = AppColors.Error,
				FontSize = DotSize * 2,
				HeightRequest = DotSize * 3.6,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
			});
		}

		if (ShowLetters && token.HasCell)
		{
			var letter = new Label
			{
				Text = token.Kind == BrailleTokenKind.Space ? " " : token.Source.ToString(),
				FontSize = 13,
				TextColor = AppColors.TextSecondary,
				HorizontalTextAlignment = TextAlignment.Center,
			};
			AutomationProperties.SetIsInAccessibleTree(letter, false);
			item.Add(letter);
		}

		SemanticProperties.SetDescription(item, DescribeToken(token));
		return item;
	}

	private static string DescribeToken(BrailleToken token) => token.Kind switch
	{
		BrailleTokenKind.Letter => $"{token.Source}, {BrailleDescriber.DescribeDots(token.Cell)}",
		BrailleTokenKind.Space => "espace",
		_ => $"{token.Source}, caractère non pris en charge"
	};
}
