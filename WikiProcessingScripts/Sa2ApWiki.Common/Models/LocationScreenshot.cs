namespace Sa2ApWiki.Common.Models;

public record LocationScreenshot
{
	public required string LocationName { get; init; }
	public required string FileName { get; init; }
	public required string LocationType { get; init; }
	public required int LocationNumber { get; init; }
	public required int ScreenshotNumber { get; init; }
	public required bool IsBonus { get; init; }
}
