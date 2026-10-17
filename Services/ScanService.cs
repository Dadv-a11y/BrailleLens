using BrailleLens.Core.Scanning;

namespace BrailleLens.Services;

public sealed class ScanService
{
	public bool IsCaptureSupported => MediaPicker.Default.IsCaptureSupported;

	public async Task<ScanResult?> CaptureAsync()
	{
		var status = await Permissions.RequestAsync<Permissions.Camera>();
		if (status != PermissionStatus.Granted)
		{
			throw new PermissionException("L'accès à la caméra a été refusé.");
		}

		var photo = await MediaPicker.Default.CapturePhotoAsync();
		if (photo is null)
		{
			return null;
		}

		try
		{
			return await AnalyzeAsync(photo);
		}
		finally
		{
			DeleteIfTemporary(photo.FullPath);
		}
	}

	public async Task<ScanResult?> PickAsync()
	{
		var photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 });
		var photo = photos?.FirstOrDefault();
		return photo is null ? null : await AnalyzeAsync(photo);
	}

	private static async Task<ScanResult> AnalyzeAsync(FileResult file)
	{
		GrayImage image;
		await using (var stream = await file.OpenReadAsync())
		{
			image = await ImageDecoder.DecodeAsync(stream, BrailleScanner.WorkingSize);
		}
		return await Task.Run(() => BrailleScanner.Recognize(image));
	}

	private static void DeleteIfTemporary(string? path)
	{
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			return;
		}

		var full = Path.GetFullPath(path);
		var temporaryRoots = new[] { FileSystem.CacheDirectory, Path.GetTempPath() }
			.Where(root => !string.IsNullOrEmpty(root))
			.Select(Path.GetFullPath);
		if (temporaryRoots.Any(root => full.StartsWith(root, StringComparison.OrdinalIgnoreCase)))
		{
			File.Delete(full);
		}
	}
}
