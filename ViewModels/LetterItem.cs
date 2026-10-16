using CommunityToolkit.Mvvm.ComponentModel;

namespace BrailleLens.ViewModels;

public partial class LetterItem : ObservableObject
{
	public LetterItem(char letter)
	{
		Letter = letter;
	}

	public char Letter { get; }

	public string Text => Letter.ToString();

	[ObservableProperty]
	private bool _isMastered;

	[ObservableProperty]
	private bool _isSelected;
}
