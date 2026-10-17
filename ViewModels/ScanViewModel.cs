using BrailleLens.Core.Braille;
using BrailleLens.Core.Scanning;
using BrailleLens.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BrailleLens.ViewModels;

public partial class ScanViewModel : ObservableObject
{
	private const string NothingFound = "Aucun point braille détecté. Essayez avec plus de lumière, de face et un peu plus près.";

	private readonly ScanService _scanner;
	private readonly NavigationService _navigation;
	private ScanResult _result = ScanResult.Empty;

	public ScanViewModel(ScanService scanner, NavigationService navigation)
	{
		_scanner = scanner;
		_navigation = navigation;
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsIdle))]
	[NotifyCanExecuteChangedFor(nameof(CaptureCommand), nameof(PickCommand))]
	private bool _isAnalyzing;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsIdle))]
	private bool _hasResult;

	[ObservableProperty]
	private string _errorMessage = string.Empty;

	[ObservableProperty]
	private bool _hasError;

	[ObservableProperty]
	private BrailleTranslation _resultTranslation = BrailleTranslator.FromCells([]);

	[ObservableProperty]
	private string _resultText = string.Empty;

	[ObservableProperty]
	private string _resultSummary = string.Empty;

	public bool IsIdle => !IsAnalyzing && !HasResult;

	public bool CanCapture => _scanner.IsCaptureSupported;

	private bool CanStart() => !IsAnalyzing;

	[RelayCommand(CanExecute = nameof(CanStart))]
	private Task Capture() => RunAsync(_scanner.CaptureAsync);

	[RelayCommand(CanExecute = nameof(CanStart))]
	private Task Pick() => RunAsync(_scanner.PickAsync);

	private async Task RunAsync(Func<Task<ScanResult?>> scan)
	{
		HasError = false;
		HasResult = false;
		IsAnalyzing = true;
		SemanticScreenReader.Announce("Analyse en cours.");
		try
		{
			var result = await scan();
			if (result is null)
			{
				return;
			}
			ShowResult(result);
		}
		catch (PermissionException exception)
		{
			ShowError(exception.Message);
		}
		catch (FeatureNotSupportedException)
		{
			ShowError("La prise de photo n'est pas disponible sur cet appareil.");
		}
		catch (Exception)
		{
			ShowError("L'image n'a pas pu être analysée. Réessayez avec une autre photo.");
		}
		finally
		{
			IsAnalyzing = false;
		}
	}

	private void ShowResult(ScanResult result)
	{
		if (!result.HasCells)
		{
			ShowError(NothingFound);
			return;
		}

		_result = result;
		ResultTranslation = BrailleTranslator.FromCells(result.Cells);
		ResultText = result.Text;
		var cells = result.Cells.Count(cell => !cell.IsEmpty);
		ResultSummary = result.UnknownCount > 0
			? $"{cells} cellules · {result.UnknownCount} non reconnue{(result.UnknownCount > 1 ? "s" : string.Empty)} (?)"
			: $"{cells} cellule{(cells > 1 ? "s" : string.Empty)} reconnue{(cells > 1 ? "s" : string.Empty)}";
		HasResult = true;
		SemanticScreenReader.Announce($"Texte reconnu : {ResultText}");
	}

	private void ShowError(string message)
	{
		ErrorMessage = message;
		HasError = true;
		SemanticScreenReader.Announce(message);
	}

	[RelayCommand]
	private void Retry()
	{
		HasResult = false;
		HasError = false;
		_result = ScanResult.Empty;
	}

	[RelayCommand]
	private Task OpenInConvert()
	{
		var unicode = new string(_result.Cells.Select(cell => cell.ToUnicode()).ToArray());
		return _navigation.GoToAsync(Routes.Convert, new Dictionary<string, object>
		{
			[ConvertViewModel.BrailleParameter] = unicode,
		});
	}
}
