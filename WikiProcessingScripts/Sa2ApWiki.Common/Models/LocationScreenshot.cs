namespace Sa2ApWiki.Common.Models;

public record LocationScreenshot
{
	public LocationScreenshot(string screenshotPath)
	{
		FileName = Path.GetFileName(screenshotPath);

		var screenshotNameWithoutExtension = Path.GetFileNameWithoutExtension(screenshotPath);
		var screenshotSplit = screenshotNameWithoutExtension.Split('-');

		LocationType = screenshotSplit[0];
		ScreenshotNumber = int.Parse(screenshotSplit[2]);
	
		var itemNumberString = screenshotSplit[1];
		LocationNumber = int.Parse(itemNumberString.Replace("bonus", ""));
		IsBonus = itemNumberString.Contains("bonus");
	}
	
	public string LocationName => $"{LocationType}-{LocationNumber}";
	public string FileName { get; }
	public string LocationType { get;  }
	public int LocationNumber { get; }
	public int ScreenshotNumber { get; }
	public bool IsBonus { get; }
}
