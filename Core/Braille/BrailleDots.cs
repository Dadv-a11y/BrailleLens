namespace BrailleLens.Core.Braille;

[Flags]
public enum BrailleDots : byte
{
	None = 0,
	Dot1 = 1 << 0,
	Dot2 = 1 << 1,
	Dot3 = 1 << 2,
	Dot4 = 1 << 3,
	Dot5 = 1 << 4,
	Dot6 = 1 << 5,
	All = Dot1 | Dot2 | Dot3 | Dot4 | Dot5 | Dot6
}
