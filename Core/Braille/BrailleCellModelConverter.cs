using System.ComponentModel;
using System.Globalization;

namespace BrailleLens.Core.Braille;

public sealed class BrailleCellModelConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
		sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
		value is string text ? Parse(text) : base.ConvertFrom(context, culture, value);

	public static BrailleCellModel Parse(string text)
	{
		var trimmed = text.Trim();
		if (trimmed.Length == 1 && char.IsLetter(trimmed[0]))
		{
			return BrailleAlphabet.GetCell(trimmed[0]);
		}

		var dots = trimmed
			.Where(char.IsDigit)
			.Select(digit => digit - '0')
			.ToArray();
		return BrailleCellModel.FromDots(dots);
	}
}
