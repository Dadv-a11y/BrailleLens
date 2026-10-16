using BrailleLens.Core.Training;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrailleLens.ViewModels;

public partial class ExerciseOption : ObservableObject
{
	public ExerciseOption(ExerciseType type, string title, string description, string iconKey)
	{
		Type = type;
		Title = title;
		Description = description;
		IconKey = iconKey;
	}

	public ExerciseType Type { get; }

	public string Title { get; }

	public string Description { get; }

	public string IconKey { get; }

	[ObservableProperty]
	private bool _isSelected;

	public static IReadOnlyList<ExerciseOption> CreateAll() =>
	[
		new(ExerciseType.RecognizeLetter, "Reconnaître une lettre", "Lire une cellule et choisir sa lettre.", "IconScanEye"),
		new(ExerciseType.FindBraille, "Trouver le Braille", "Associer une lettre à sa cellule.", "IconGrid"),
		new(ExerciseType.WriteBraille, "Écrire en Braille", "Composer une cellule point par point.", "IconPencil"),
		new(ExerciseType.ConvertWord, "Convertir un mot", "Écrire un mot avec plusieurs cellules.", "IconSwap"),
	];
}
