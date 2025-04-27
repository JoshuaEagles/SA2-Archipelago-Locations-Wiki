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
	var relevantChronologicalLocationsForStage = chronologicalLocations[stageName]
		.Where(x => x.LocationName.Contains("pipe") || x.LocationName.Contains("hidden") || x.LocationName.Contains("animal"))
		.ToImmutableArray();

	var locationPaths = Directory.EnumerateFiles(directoryPath, "*.webp", SearchOption.AllDirectories);
	
	var pngScreenshots = Directory.EnumerateFiles(directoryPath, $"*.png", SearchOption.TopDirectoryOnly);
	var webpScreenshots = Directory.EnumerateFiles(directoryPath, $"*.webp", SearchOption.TopDirectoryOnly);
	var locationsScreenshotsLookup = pngScreenshots.Concat(webpScreenshots)
		.Select(screenshotPath => new LocationScreenshot(screenshotPath))
		.ToLookup(x => x.LocationName);
	
	foreach (var locationScreenshotGroup in locationsScreenshotsLookup)
	{
		foreach (var locationScreenshot in locationScreenshotGroup)
		{
			var filePath = $"{directoryPath}/{locationScreenshot.FileName}";
			if (locationScreenshot.LocationType == "animal" && locationScreenshot.IsBonus == false)
			{
				var chronologicalIndexOfAnimal =
					FindIndex(relevantChronologicalLocationsForStage, locationScreenshot.LocationName);

				if (chronologicalIndexOfAnimal == 0)
				{
					continue;
				}
				
				var chronologicalLocationBeforeChronologicalAnimal = relevantChronologicalLocationsForStage[chronologicalIndexOfAnimal - 1];

				if (!chronologicalLocationBeforeChronologicalAnimal.LocationName.Contains("pipe") &&
				    !chronologicalLocationBeforeChronologicalAnimal.LocationName.Contains("hidden"))
				{
					continue;
				}
				
				var screenshotSplit = chronologicalLocationBeforeChronologicalAnimal.LocationName.Split('-');
				var linkedLocationType = screenshotSplit[0];
				var linkedLocationNumber = screenshotSplit[1];
				
				var newFileName =
					$"{linkedLocationType}-{linkedLocationNumber}-{locationScreenshot.ScreenshotNumber}.webp";
			
				Console.WriteLine(
					$"Copying {filePath} to {directoryPath}/{newFileName}");
				File.Copy(filePath, $"{directoryPath}/{newFileName}");
			}
		}


		// File.Move($"{directoryPath}/{locationScreenshot.FileName}", $"{directoryPath}/{newFileName}");
	}
}

int FindIndex(ImmutableArray<ChronologicalLocationModel> chronologicalLocationsForStage, string locationName)
{
	foreach (var chronologicalLocation in chronologicalLocationsForStage)
	{
		if (chronologicalLocation.LocationName == locationName)
		{
			return chronologicalLocationsForStage.IndexOf(chronologicalLocation);
		}
	}
	
	throw new ArgumentException("Not found");
}
