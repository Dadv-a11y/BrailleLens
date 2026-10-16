using BrailleLens.Controls;
using BrailleLens.Core.Braille;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrailleLens.ViewModels;

public partial class ChoiceItem : ObservableObject
{
	public ChoiceItem(int index)
	{
		Index = index;
		Caption = $"Choix {(char)('A' + index)}";
	}

	public int Index { get; }

	public string Caption { get; }

	[ObservableProperty]
	private char _letter;

	[ObservableProperty]
	private ChoiceState _state;

	public string LetterText => Letter.ToString();

	public BrailleCellModel Cell => Letter == default ? BrailleCellModel.Empty : BrailleAlphabet.GetCell(Letter);

	partial void OnLetterChanged(char value)
	{
		OnPropertyChanged(nameof(LetterText));
		OnPropertyChanged(nameof(Cell));
	}
}
