using BrailleLens.Core.Braille;
using Microsoft.Maui.Layouts;

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

	private readonly FlexLayout _layout = new()
	{
		Wrap = FlexWrap.Wrap,
		Direction = FlexDirection.Row,
		JustifyContent = FlexJustify.Start,
		AlignItems = FlexAlignItems.Start,
	};

	public BrailleStrip()
	{
		Content = _layout;
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

	private static void OnLayoutChanged(BindableObject bindable, object oldValue, object newValue) =>
		((BrailleStrip)bindable).BuildLayout();

	private void BuildLayout()
	{
		_layout.Clear();
		var tokens = Translation?.Tokens ?? Array.Empty<BrailleToken>();
		var margin = new Thickness(0, 0, DotSize * 1.2, DotSize);

		foreach (var token in tokens)
		{
			var item = new VerticalStackLayout { Spacing = DotSize * 0.6, Margin = margin };
			if (token.HasCell)
			{
				var cell = new BrailleCell { Cell = token.Cell, DotSize = DotSize };
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
			_layout.Add(item);
		}
	}

	private static string DescribeToken(BrailleToken token) => token.Kind switch
	{
		BrailleTokenKind.Letter => $"{token.Source}, {BrailleDescriber.DescribeDots(token.Cell)}",
		BrailleTokenKind.Space => "espace",
		_ => $"{token.Source}, caractère non pris en charge"
	};
}
