using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Models;

namespace Sa2ApWiki.MarkdownPageGeneratorScript;

public static class MarkdownGenerator
{
    public static void GenerateMarkdownFiles(string path)
    {
        foreach (var characterName in Constants.CharacterNames)
        {
            // Go through each directory in the character folders
            // For each one not named Chronological:
            // Generate a file based on the Folder name, <Name>Itemsanity.md
        
            // In the file: 
            // First line is <Name> (Itemsanity)
            // Then an empty line, followed by:
        
            // ## <Name> Item n
            // ![](<path-to-screenshot>)
            // repeat for each one
            // Empty line
            // [Back to Top](#)
            // Empty line
        
            // Same idea for Lives
        
            foreach (var directoryPath in Directory.EnumerateDirectories(Path.Join(path, characterName), string.Empty, SearchOption.TopDirectoryOnly))
            {
                if (directoryPath.Contains("Chronological"))
                {
                    continue;
                }

                var newFilePath = $"{directoryPath}Itemsanity.md";
                using var streamWriter = new StreamWriter(new FileStream(newFilePath, FileMode.Create, FileAccess.Write));

                var stageName = Path.GetFileName(directoryPath);
            
                streamWriter.WriteLine($"# {stageName} (Itemsanity)");
                streamWriter.WriteLine();

                WriteMarkdownLocationsOfSingleType("Item", "item", directoryPath, streamWriter, stageName);
                WriteMarkdownLocationsOfSingleType("Life", "life", directoryPath, streamWriter, stageName);
            }
        }

    }

    private static void WriteMarkdownLocationsOfSingleType(string readableLocationTypeName, string locationShortName, string directoryPath, StreamWriter streamWriter, string stageName)
    {
        var locations = Directory.EnumerateFiles(directoryPath, $"{locationShortName}*", SearchOption.TopDirectoryOnly)
            .Select(screenshotPath => new LocationScreenshot(screenshotPath))
            .ToList();

        var locationsByItemNumber = locations
            .GroupBy(l => l.LocationNumber)
            .OrderBy(l => l.Key);

        foreach (var locationGroup in locationsByItemNumber)
        {
            streamWriter.WriteLine($"## {stageName} {readableLocationTypeName} {locationGroup.Key}");
            
            var sortedLocationGroup = locationGroup.OrderBy(l => l.ScreenshotNumber);
            foreach (var location in sortedLocationGroup)
            {
                streamWriter.WriteLine($"![](./{stageName}/{location.FileName})");
            }
                
            streamWriter.WriteLine();
            streamWriter.WriteLine("[Back to Top](#)");
            streamWriter.WriteLine();
        }
    }
}
