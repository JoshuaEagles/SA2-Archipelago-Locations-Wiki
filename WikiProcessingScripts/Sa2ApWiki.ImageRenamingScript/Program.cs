using System.Collections.Immutable;
using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Models;
using Sa2ApWiki.Common.Services;

var path = "/home/entiss/Projects/SA2-Archipelago-Locations-Wiki/";

var chronologicalLocationDocumentPath = Path.Join(path, "ChronologicalLocationOrder.txt");
var chronologicalLocations = ChronologicalLocationOrderParser.Parse(chronologicalLocationDocumentPath);

foreach (var characterName in Constants.CharacterNames)
{
	// Go through each directory in the character folders
	// For each one not named Chronological:
	// Generate a file based on the Folder name, <Name>Chronological.md
        
	// In the file: 
            
	// ## <Name> Item n
	// ![](<path-to-screenshot>)
	// repeat for each one
	// Empty line
        
	// Same idea for Lives
        
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
		
		var newFileName =
			$"{stageName}-{locationScreenshot.LocationType}-{(locationScreenshot.IsBonus ? "bonus" : "")}{locationScreenshot.LocationNumber}-{locationScreenshot.ScreenshotNumber}{Path.GetExtension(filePath)}";
		
		Console.WriteLine(
			$"Renaming {filePath} to {directoryPath}/{newFileName}");
		File.Move(filePath, $"{directoryPath}/{newFileName}");
	}
}
