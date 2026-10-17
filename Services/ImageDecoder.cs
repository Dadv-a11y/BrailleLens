using BrailleLens.Core.Scanning;

namespace BrailleLens.Services;

public static partial class ImageDecoder
{
	public static partial Task<GrayImage> DecodeAsync(Stream stream, int maxSide);
}
