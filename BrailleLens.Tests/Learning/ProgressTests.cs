using System.Text.Json;
using BrailleLens.Core.Learning;

namespace BrailleLens.Tests.Learning;

public class ProgressTests
{
	[Fact]
	public void New_progress_is_at_first_use()
	{
		var progress = new Progress();

		Assert.False(progress.HasStarted);
		Assert.Equal('A', progress.CurrentLetter);
		Assert.Equal(0, progress.MasteredCount);
		Assert.Equal(1, progress.Level);
	}

	[Fact]
	public void Letter_is_mastered_after_threshold_correct_answers()
	{
		var progress = new Progress();

		Assert.False(progress.RecordAnswer('b', true));
		Assert.False(progress.IsMastered('B'));
		Assert.True(progress.RecordAnswer('B', true));
		Assert.True(progress.IsMastered('b'));
		Assert.False(progress.RecordAnswer('B', true));
		Assert.Equal(1, progress.MasteredCount);
	}

	[Fact]
	public void Wrong_answers_do_not_count()
	{
		var progress = new Progress();

		progress.RecordAnswer('C', false);
		progress.RecordAnswer('C', false);

		Assert.False(progress.IsMastered('C'));
		Assert.True(progress.HasStarted);
	}

	[Fact]
	public void Level_rises_with_mastered_letters()
	{
		var progress = Master("ABCDEFGH");

		Assert.Equal(8, progress.MasteredCount);
		Assert.Equal(2, progress.Level);
		Assert.Equal("ABCDEFGH", new string(progress.MasteredLetters.ToArray()));
	}

	[Fact]
	public void Unlocked_letters_follow_level_and_current_letter()
	{
		var progress = new Progress();
		Assert.Equal(6, progress.UnlockedLetters.Count);

		progress.SelectLetter('j');
		Assert.Equal('J', progress.CurrentLetter);
		Assert.Equal(10, progress.UnlockedLetters.Count);
	}

	[Fact]
	public void SelectLetter_rejects_unsupported_letters()
	{
		Assert.Throws<ArgumentException>(() => new Progress().SelectLetter('é'));
	}

	[Fact]
	public void Reset_returns_to_first_use()
	{
		var progress = Master("ABC");
		progress.SelectLetter('D');

		progress.Reset();

		Assert.False(progress.HasStarted);
		Assert.Equal('A', progress.CurrentLetter);
		Assert.Equal(0, progress.MasteredCount);
	}

	[Fact]
	public void Snapshot_survives_json_round_trip()
	{
		var progress = Master("AB");
		progress.RecordAnswer('C', true);
		progress.SelectLetter('E');

		var json = JsonSerializer.Serialize(progress.ToSnapshot());
		var restored = Progress.FromSnapshot(JsonSerializer.Deserialize<ProgressSnapshot>(json));

		Assert.True(restored.HasStarted);
		Assert.Equal('E', restored.CurrentLetter);
		Assert.Equal(2, restored.MasteredCount);
		Assert.True(restored.RecordAnswer('C', true));
	}

	[Fact]
	public void FromSnapshot_ignores_invalid_data()
	{
		var snapshot = new ProgressSnapshot(true, '#', new Dictionary<char, int> { ['é'] = 5, ['a'] = 3, ['B'] = -1 });

		var progress = Progress.FromSnapshot(snapshot);

		Assert.Equal('A', progress.CurrentLetter);
		Assert.Equal(new[] { 'A' }, progress.MasteredLetters);
		Assert.Equal(1, Progress.FromSnapshot(null).Level);
	}

	private static Progress Master(string letters)
	{
		var progress = new Progress();
		foreach (var letter in letters)
		{
			for (var i = 0; i < Progress.MasteryThreshold; i++)
			{
				progress.RecordAnswer(letter, true);
			}
		}
		return progress;
	}
}
