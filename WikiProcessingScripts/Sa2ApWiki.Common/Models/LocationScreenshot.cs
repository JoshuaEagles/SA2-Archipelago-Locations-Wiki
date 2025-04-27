namespace Sa2ApWiki.Common.Models;

public record LocationScreenshot
{
	public LocationScreenshot(string screenshotPath)
	{
		FileName = Path.GetFileName(screenshotPath);

		var screenshotNameWithoutExtension = Path.GetFileNameWithoutExtension(screenshotPath);
		var screenshotSplit = screenshotNameWithoutExtension.Split('-');

		StageName = screenshotSplit[0];
		LocationType = screenshotSplit[1];
		ScreenshotNumber = int.Parse(screenshotSplit[3]);
	
		var itemNumberString = screenshotSplit[2];
		LocationNumber = int.Parse(itemNumberString.Replace("bonus", ""));
		IsBonus = itemNumberString.Contains("bonus");
	}
	
	public string LocationName => $"{LocationType}-{LocationNumber}";
	public string FileName { get; }
	public string StageName { get; }
	public string LocationType { get;  }
	public int LocationNumber { get; }
	public int ScreenshotNumber { get; }
	public bool IsBonus { get; }
}
