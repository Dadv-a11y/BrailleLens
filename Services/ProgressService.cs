using BrailleLens.Core.Learning;

namespace BrailleLens.Services;

public sealed class ProgressService
{
	private readonly IProgressStore _store;
	private Progress? _progress;

	public ProgressService(IProgressStore store)
	{
		_store = store;
	}

	public event EventHandler? Changed;

	public Progress Progress => _progress ??= _store.Load();

	public void Save()
	{
		_store.Save(Progress);
		Changed?.Invoke(this, EventArgs.Empty);
	}

	public void Reset()
	{
		Progress.Reset();
		Save();
	}
}
