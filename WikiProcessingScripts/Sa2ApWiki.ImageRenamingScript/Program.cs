using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Models;

const string path = "/home/entiss/Projects/SA2-Archipelago-Locations-Wiki/";

foreach (var characterName in Constants.CharacterNames)
{
	foreach (var directoryPath in Directory.EnumerateDirectories(Path.Join(path, characterName), string.Empty, SearchOption.TopDirectoryOnly))
	{
		if (directoryPath.Contains("Chronological"))
		{
			continue;
		}

		var stageName = Path.GetFileName(directoryPath);
		
		PerformFileRename(stageName, directoryPath);
	}
}

return;

void PerformFileRename(string stageName, string directoryPath)
{
	var locationScreenshots = Directory.EnumerateFiles(directoryPath, "*.webp", SearchOption.AllDirectories)
		.Concat(Directory.EnumerateFiles(directoryPath, $"*.png", SearchOption.TopDirectoryOnly))
		.Select(screenshotPath => new LocationScreenshot(screenshotPath));
	
	foreach (var locationScreenshot in locationScreenshots)
	{
		var filePath = $"{directoryPath}/{locationScreenshot.FileName}";
		
		var zeroPaddedLocationNumber = Helpers.ZeroPadNumber(locationScreenshot.LocationNumber, 2);
		
		var newFileName =
			$"{stageName}-{locationScreenshot.LocationType}-{(locationScreenshot.IsBonus ? "bonus" : "")}{zeroPaddedLocationNumber}-{locationScreenshot.ScreenshotNumber}{Path.GetExtension(filePath)}";
		
		Console.WriteLine(
			$"Renaming {filePath} to {directoryPath}/{newFileName}");
		File.Move(filePath, $"{directoryPath}/{newFileName}");
	}
}
