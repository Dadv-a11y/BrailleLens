namespace BrailleLens.Controls;

public static class AppColors
{
	public static Color Get(string key) =>
		Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
			? color
			: Colors.Magenta;

	public static Color Primary => Get("Primary");
	public static Color PrimaryPressed => Get("PrimaryPressed");
	public static Color PrimarySoft => Get("PrimarySoft");
	public static Color PrimarySoftPressed => Get("PrimarySoftPressed");
	public static Color OnPrimary => Get("OnPrimary");
	public static Color Background => Get("Background");
	public static Color Surface => Get("Surface");
	public static Color Border => Get("Border");
	public static Color DotInactive => Get("DotInactive");
	public static Color Overlay => Get("Overlay");
	public static Color TextPrimary => Get("TextPrimary");
	public static Color TextSecondary => Get("TextSecondary");
	public static Color TextDisabled => Get("TextDisabled");
	public static Color Success => Get("Success");
	public static Color SuccessSoft => Get("SuccessSoft");
	public static Color Error => Get("Error");
	public static Color ErrorSoft => Get("ErrorSoft");

	public static string Icon(string key) =>
		Application.Current?.Resources.TryGetValue(key, out var value) == true && value is string data
			? data
			: string.Empty;
}
