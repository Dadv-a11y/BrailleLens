namespace BrailleLens.Core.Training;

public static class WordBank
{
	public static IReadOnlyList<string> Words { get; } =
	[
		"BAC", "BEC", "FACE", "CAGE", "AIDE", "BAIE", "CHEF", "AMI", "AMIE", "LAC",
		"BAL", "BOL", "CIEL", "DAME", "JEU", "MAIN", "FILM", "LIRE", "LUNE", "MER",
		"MOT", "NID", "PAIN", "ROBE", "ROI", "SAC", "SEL", "SOL", "GARE", "JOUR",
		"NUIT", "PORTE", "TABLE", "CHAT", "LIVRE", "VIE", "DOUX", "VOIX", "YEUX", "ZOO",
		"QUAI", "HIBOU", "BIJOU", "KIWI", "WAGON"
	];

	public static string Pick(IReadOnlyCollection<char> knownLetters, Random random, string? exclude = null)
	{
		var known = knownLetters.Select(char.ToUpperInvariant).ToHashSet();
		var candidates = Words.Where(word => word != exclude).ToArray();
		var fewestUnknown = candidates.Min(word => CountUnknown(word, known));
		var best = candidates.Where(word => CountUnknown(word, known) == fewestUnknown).ToArray();
		return best[random.Next(best.Length)];
	}

	private static int CountUnknown(string word, HashSet<char> known) => word.Distinct().Count(letter => !known.Contains(letter));
}
