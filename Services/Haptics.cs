namespace BrailleLens.Services;

public static class Haptics
{
	public static bool IsEnabled { get; set; } = true;

	public static void Tap() => Perform(HapticFeedbackType.Click);

	public static void Alert() => Perform(HapticFeedbackType.LongPress);

	private static void Perform(HapticFeedbackType type)
	{
		if (!IsEnabled || !HapticFeedback.Default.IsSupported)
		{
			return;
		}

		try
		{
			HapticFeedback.Default.Perform(type);
		}
		catch (FeatureNotSupportedException)
		{
		}
	}
}
