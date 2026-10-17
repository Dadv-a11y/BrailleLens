using BrailleLens.Core.Scanning;

namespace BrailleLens.Tests.Scanning;

public class BrailleScannerTests
{
	[Theory]
	[InlineData("BONJOUR")]
	[InlineData("AMI")]
	[InlineData("KIWI")]
	[InlineData("ZOO")]
	[InlineData("A")]
	public void Recognizes_clean_words(string word)
	{
		var result = BrailleScanner.Recognize(SyntheticBraille.Render(word));

		Assert.Equal(word, result.Text);
	}

	[Fact]
	public void Recognizes_full_alphabet()
	{
		const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		var result = BrailleScanner.Recognize(SyntheticBraille.Render(alphabet, dotPitch: 12));

		Assert.Equal(alphabet, result.Text);
	}

	[Fact]
	public void Detects_spaces_between_words()
	{
		var result = BrailleScanner.Recognize(SyntheticBraille.Render("BON JOUR"));

		Assert.Equal("BON JOUR", result.Text);
	}

	[Theory]
	[InlineData(10)]
	[InlineData(35)]
	public void Handles_scale_changes(double dotPitch)
	{
		var result = BrailleScanner.Recognize(SyntheticBraille.Render("SALUT", dotPitch));

		Assert.Equal("SALUT", result.Text);
	}

	[Fact]
	public void Tolerates_noise()
	{
		var result = BrailleScanner.Recognize(SyntheticBraille.Render("MERCI", noise: 25, seed: 3));

		Assert.Equal("MERCI", result.Text);
	}

	[Fact]
	public void Reads_light_dots_on_dark_background()
	{
		var result = BrailleScanner.Recognize(SyntheticBraille.Render("LIRE", inverted: true));

		Assert.Equal("LIRE", result.Text);
	}

	[Fact]
	public void Reads_image_photographed_sideways()
	{
		var image = SyntheticBraille.Render("PAIN").Rotate90();

		var result = BrailleScanner.Recognize(image);

		Assert.Equal("PAIN", result.Text);
	}

	[Fact]
	public void Blank_image_gives_no_cells()
	{
		var image = new GrayImage(200, 100, Enumerable.Repeat((byte)200, 200 * 100).ToArray());

		var result = BrailleScanner.Recognize(image);

		Assert.False(result.HasCells);
		Assert.Equal(string.Empty, result.Text);
	}

	[Fact]
	public void Downscale_keeps_aspect_and_limits_size()
	{
		var image = SyntheticBraille.Render("BONJOUR", dotPitch: 60);

		var small = image.Downscale(400);

		Assert.True(Math.Max(small.Width, small.Height) <= 400);
		Assert.Equal(image.Width / (double)image.Height, small.Width / (double)small.Height, 1);
	}

	[Fact]
	public void FromRgba_converts_to_luminance()
	{
		var image = GrayImage.FromRgba(2, 1, new byte[] { 255, 255, 255, 255, 0, 0, 0, 255 });

		Assert.Equal(255, image[0, 0]);
		Assert.Equal(0, image[1, 0]);
	}
}
