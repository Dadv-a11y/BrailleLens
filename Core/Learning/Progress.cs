using BrailleLens.Core.Braille;

namespace BrailleLens.Core.Learning;

public sealed record ProgressSnapshot(bool HasStarted, char CurrentLetter, Dictionary<char, int> CorrectAnswers);

public sealed class Progress
{
	public const int MasteryThreshold = 2;

	private readonly Dictionary<char, int> _correctAnswers = new();

	public bool HasStarted { get; private set; }

	public char CurrentLetter { get; private set; } = BrailleAlphabet.Letters[0];

	public int MasteredCount => _correctAnswers.Count(pair => pair.Value >= MasteryThreshold);

	public int Level => LevelPolicy.GetLevel(MasteredCount);

	public IReadOnlyList<char> MasteredLetters =>
		BrailleAlphabet.Letters.Where(IsMastered).ToArray();

	public IReadOnlyList<char> UnlockedLetters
	{
		get
		{
			var count = Math.Max(
				LevelPolicy.UnlockedLetterCount(Level),
				BrailleAlphabet.IndexOf(CurrentLetter) + 1);
			return BrailleAlphabet.Letters.Take(count).ToArray();
		}
	}

	public bool IsMastered(char letter) =>
		_correctAnswers.TryGetValue(char.ToUpperInvariant(letter), out var count) && count >= MasteryThreshold;

	public void Start()
	{
		HasStarted = true;
	}

	public void SelectLetter(char letter)
	{
		if (!BrailleAlphabet.IsSupported(letter))
		{
			throw new ArgumentException($"La lettre « {letter} » n'est pas prise en charge.", nameof(letter));
		}
		HasStarted = true;
		CurrentLetter = char.ToUpperInvariant(letter);
	}

	public bool RecordAnswer(char letter, bool correct)
	{
		HasStarted = true;
		if (!correct || !BrailleAlphabet.IsSupported(letter))
		{
			return false;
		}

		var key = char.ToUpperInvariant(letter);
		var wasMastered = IsMastered(key);
		_correctAnswers[key] = _correctAnswers.GetValueOrDefault(key) + 1;
		return !wasMastered && IsMastered(key);
	}

	public void Reset()
	{
		_correctAnswers.Clear();
		HasStarted = false;
		CurrentLetter = BrailleAlphabet.Letters[0];
	}

	public ProgressSnapshot ToSnapshot() => new(HasStarted, CurrentLetter, new Dictionary<char, int>(_correctAnswers));

	public static Progress FromSnapshot(ProgressSnapshot? snapshot)
	{
		var progress = new Progress();
		if (snapshot is null)
		{
			return progress;
		}

		progress.HasStarted = snapshot.HasStarted;
		if (BrailleAlphabet.IsSupported(snapshot.CurrentLetter))
		{
			progress.CurrentLetter = char.ToUpperInvariant(snapshot.CurrentLetter);
		}
		foreach (var (letter, count) in snapshot.CorrectAnswers ?? new Dictionary<char, int>())
		{
			if (BrailleAlphabet.IsSupported(letter) && count > 0)
			{
				progress._correctAnswers[char.ToUpperInvariant(letter)] = count;
			}
		}
		return progress;
	}
}
