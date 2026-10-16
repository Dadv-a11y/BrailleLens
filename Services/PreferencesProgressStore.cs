using System.Text.Json;
using BrailleLens.Core.Learning;

namespace BrailleLens.Services;

public sealed class PreferencesProgressStore : IProgressStore
{
	private const string Key = "progress";

	private readonly IPreferences _preferences;

	public PreferencesProgressStore(IPreferences preferences)
	{
		_preferences = preferences;
	}

	public Progress Load()
	{
		var json = _preferences.Get(Key, string.Empty);
		if (string.IsNullOrEmpty(json))
		{
			return new Progress();
		}

		try
		{
			return Progress.FromSnapshot(JsonSerializer.Deserialize<ProgressSnapshot>(json));
		}
		catch (JsonException)
		{
			return new Progress();
		}
	}

	public void Save(Progress progress) =>
		_preferences.Set(Key, JsonSerializer.Serialize(progress.ToSnapshot()));
}
