using Sa2ApWiki.Common.Models;

// Console.WriteLine("Enter path to root of project:");
// var path = Console.ReadLine();
var path = "/home/entiss/Projects/SA2-Archipelago-Locations-Wiki/";

if (path is null || path == string.Empty) {
	throw new Exception("Invalid path");
}

PerformFileRename(Directory.EnumerateFiles(path, "*.webp", SearchOption.AllDirectories));
PerformFileRename(Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories));
return;

void PerformFileRename(IEnumerable<string> filePaths)
{
	foreach (var filePath in filePaths)
	{
		var locationScreenshot = CreateLocationScreenshotModel(filePath);

		if (locationScreenshot.LocationType == "goldbeetle" || locationScreenshot.LocationType == "lostchao" ||
		    locationScreenshot.LocationType == "upgrade")
		{
			var newFileName =
				$"{locationScreenshot.LocationType}-{locationScreenshot.LocationNumber}-{locationScreenshot.ScreenshotNumber}.webp";

			var fileDirectory = Path.GetDirectoryName(filePath);
			Console.WriteLine(
				$"Renaming {fileDirectory}/{locationScreenshot.FileName} to {fileDirectory}/{newFileName}");

			File.Move($"{fileDirectory}/{locationScreenshot.FileName}", $"{fileDirectory}/{newFileName}");
		}
	}
}

LocationScreenshot CreateLocationScreenshotModel(string screenshotPath)
{
	var screenshotNameWithoutExtension = Path.GetFileNameWithoutExtension(screenshotPath);
	var screenshotNameWithExtension = Path.GetFileName(screenshotPath);

	var screenshotSplit = screenshotNameWithoutExtension.Split('-');

	var itemType = screenshotSplit[0];
	var itemNumber = 1;
	
	int screenshotNumber;
	bool isBonus;
	if (itemType == "goldbeetle" || itemType == "lostchao" || itemType == "upgrade")
	{
		isBonus = false;
		screenshotNumber = int.Parse(screenshotSplit[1]);
	}
	else 
	{
		var itemNumberString = screenshotSplit[1];
		itemNumber = int.Parse(itemNumberString.Replace("bonus", ""));
		isBonus = itemNumberString.Contains("bonus");
		
		screenshotNumber = int.Parse(screenshotSplit[2]);
	}

	return new LocationScreenshot
	{
		LocationName = $"{itemType}-{itemNumber}",
		FileName = screenshotNameWithExtension,
		LocationType = itemType,
		LocationNumber = itemNumber,
		ScreenshotNumber = screenshotNumber,
		IsBonus = isBonus
	};
}
