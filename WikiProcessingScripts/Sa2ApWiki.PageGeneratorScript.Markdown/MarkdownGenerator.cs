using System.Collections.Immutable;
using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Models;

// Need to generate basic chronological pages so that I can add itemsanity and bigsanity to them
// Then I need to update the chronological parser to generate in the new nicer format, maybe put it into a JSON file too

namespace Sa2ApWiki.MarkdownPageGeneratorScript;

public class MarkdownGenerator
{
    private readonly Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>> _chronologicalLocationsByStage;
    
    public MarkdownGenerator(Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>> chronologicalLocationsByStage)
    {
        _chronologicalLocationsByStage = chronologicalLocationsByStage;
    }
    
    public void GenerateMarkdownFiles(string path)
    {
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

                var chronologicalFilePath = $"{directoryPath}Chronological.md";
                using (var streamWriter =
                       new StreamWriter(new FileStream(chronologicalFilePath, FileMode.Create, FileAccess.Write)))
                {
                    streamWriter.WriteLine("<style>img{width:256px;display:inline;}</style>");

                    var stageName = Path.GetFileName(directoryPath);
            
                    WriteChronologicalMarkdownLocations(stageName, directoryPath, streamWriter);
                }
                
                var itemsanityBigsanityFilePath = $"{directoryPath}ItemsanityBigsanity.md";
                using (var streamWriter =
                       new StreamWriter(new FileStream(itemsanityBigsanityFilePath, FileMode.Create, FileAccess.Write)))
                {
                    streamWriter.WriteLine("<style>img{width:256px;display:inline;}</style>");

                    var stageName = Path.GetFileName(directoryPath);
            
                    WriteSimplifiedItemsanityAndBigsanityPages(stageName, directoryPath, streamWriter);
                }
            }
        }

    }

    private void WriteChronologicalMarkdownLocations(string stageName, string directoryPath, StreamWriter streamWriter)
    {
        var pngScreenshots = Directory.EnumerateFiles(directoryPath, $"*.png", SearchOption.TopDirectoryOnly);
        var webpScreenshots = Directory.EnumerateFiles(directoryPath, $"*.webp", SearchOption.TopDirectoryOnly);
        
        var locationsScreenshotsLookup = pngScreenshots.Concat(webpScreenshots)
            .Select(screenshotPath => new LocationScreenshot(screenshotPath))
            .ToLookup(x => x.LocationName);

        var locationsChronologicallyOrdered = _chronologicalLocationsByStage[stageName];

        foreach (var chronologicalLocation in locationsChronologicallyOrdered)
        {
            var locationScreenshotsGroup = locationsScreenshotsLookup[chronologicalLocation.LocationName].ToImmutableArray();
            
            var locationType = locationScreenshotsGroup.First().LocationType;
            var locationNumber = locationScreenshotsGroup.First().LocationNumber;
            
            streamWriter.WriteLine($"## {locationType} {locationNumber}");
            
            var sortedLocationGroup = locationScreenshotsGroup.OrderBy(x => x.ScreenshotNumber);
            foreach (var location in sortedLocationGroup)
            {
                streamWriter.WriteLine($"![](./{stageName}/{location.FileName})");
            }
                
            streamWriter.WriteLine();
        }
    }

    private void WriteSimplifiedItemsanityAndBigsanityPages(string stageName, string directoryPath, StreamWriter streamWriter)
    {
        var pngScreenshots = Directory.EnumerateFiles(directoryPath, $"*.png", SearchOption.TopDirectoryOnly);
        var webpScreenshots = Directory.EnumerateFiles(directoryPath, $"*.webp", SearchOption.TopDirectoryOnly);

        var locationsScreenshots = pngScreenshots.Concat(webpScreenshots)
            .Where(x => x.Contains("item") || x.Contains("life") || x.Contains("big"))
            .Select(screenshotPath => new LocationScreenshot(screenshotPath))
            .GroupBy(x => x.LocationName)
            .OrderBy(x => x.First().LocationType)
            .ThenBy(x => x.First().LocationNumber);

        foreach (var locationScreenshotsGroup in locationsScreenshots)
        {
            var locationType = locationScreenshotsGroup.First().LocationType;
            var locationNumber = locationScreenshotsGroup.First().LocationNumber;
            
            streamWriter.WriteLine($"## {locationType} {locationNumber}");
            
            var sortedLocationGroup = locationScreenshotsGroup.OrderBy(x => x.ScreenshotNumber);
            foreach (var location in sortedLocationGroup)
            {
                streamWriter.WriteLine($"![](./{stageName}/{location.FileName})");
            }
                
            streamWriter.WriteLine();
        }
    }
}
