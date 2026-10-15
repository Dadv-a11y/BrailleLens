namespace BrailleLens.Core.Learning;

public interface IProgressStore
{
	Progress Load();

	void Save(Progress progress);
}
