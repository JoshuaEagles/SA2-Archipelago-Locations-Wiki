using System.Collections.Immutable;
using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Models;

namespace Sa2ApWiki.MirahezePageGeneratorScript;

public class MirahezeGenerator
{
    private Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>> _chronologicalLocationsByStage;
    
    public MirahezeGenerator(Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>> chronologicalLocationsByStage)
    {
        _chronologicalLocationsByStage = chronologicalLocationsByStage;
    }
    
    public void GenerateMirahezeFiles(string path)
    {
        foreach (var characterName in Constants.CharacterNames)
        {
            if (characterName != "Sonic") continue;
            
            foreach (var directoryPath in Directory.EnumerateDirectories(Path.Join(path, characterName), string.Empty, SearchOption.TopDirectoryOnly))
            {
                if (directoryPath.Contains("Chronological"))
                {
                    continue;
                }

                var newFilePath = $"{directoryPath}Miraheze.txt";
                using var streamWriter = new StreamWriter(new FileStream(newFilePath, FileMode.Create, FileAccess.Write));

                var stageName = Path.GetFileName(directoryPath);
                
                var chronologicalFirstLocation = _chronologicalLocationsByStage[stageName].First().LocationName;
            
                WriteStagePageHeader(stageName, streamWriter, chronologicalFirstLocation);
                WriteAllLocationToStagePage(directoryPath, streamWriter, stageName);
                WriteStagePageFooter(streamWriter);
            }
        }

    }

    private static void WriteStagePageHeader(string stageName, StreamWriter streamWriter, string chronologicalFirstLocation)
    {
        var readableStageName = Constants.StageCodeNameToReadableName[stageName];
        var headerText = $$$"""
                         __NOTOC__{{DISPLAYTITLE:{{{readableStageName}}} Locations}}
                         Display as Chronological:
                         <div id="chronological-toggle">button</div>
                         
                         <div id="chronological-first-location" data-chronological-first-location="{{{chronologicalFirstLocation}}}"></div>

                         <div id="location-wrapper">
                             
                         """;
        
        streamWriter.WriteLine(headerText);
    }

    private void WriteAllLocationToStagePage(string directoryPath, StreamWriter streamWriter, string stageName)
    {
        var chronologicalLocationsByLocationName = GetChronologicalLocationsByLocationName(stageName);

        var locationsLookupByLocationType = GetLocationsLookupByLocationType(directoryPath);

        foreach (var locationType in Constants.LocationTypes)
        {
            var sortedLocationsOfType = locationsLookupByLocationType[locationType.CodeName]
                .GroupBy(y => y.LocationNumber)
                .OrderBy(y => y.Key);
            
            // The locationGroup represents a single location
            // Each entry in it is a different screenshot for that location
            foreach (var locationGroup in sortedLocationsOfType)
            {
                var currentLocationName = $"{locationType.CodeName}-{locationGroup.Key}";
                var nextChronologicalLocationName =
                    chronologicalLocationsByLocationName[currentLocationName].NextLocation?.LocationName;
                
                streamWriter.WriteLine($"""
                                        <div 
                                            id="{locationType.CodeName}-{locationGroup.Key}" 
                                            class="location" 
                                            data-type="{locationType.CodeName}" 
                                        """
                    );
                if (nextChronologicalLocationName is not null)
                {
                    streamWriter.WriteLine($"""    data-chronological-next-location="{nextChronologicalLocationName}""");
                }
                streamWriter.WriteLine(">");
                
                streamWriter.WriteLine($"=== {locationType.ReadableName} {locationGroup.Key} {locationType.LocationNameSuffix} ===");
                
                var sortedLocationGroup = locationGroup.OrderBy(l => l.ScreenshotNumber);
                foreach (var location in sortedLocationGroup)
                {
                    streamWriter.Write($"[[Image:{location.FileName}|512px|link=]] ");
                }
                streamWriter.WriteLine("<br />");
                
                streamWriter.WriteLine("[[#top|Back to top]]");
                streamWriter.WriteLine("</div>");
                streamWriter.WriteLine();
            }
        }
    }

    private static ILookup<string, LocationScreenshot> GetLocationsLookupByLocationType(string directoryPath)
    {
        var allLocationsForStage = Directory.EnumerateFiles(directoryPath, $"*", SearchOption.TopDirectoryOnly)
            .Select(screenshotPath => new LocationScreenshot(screenshotPath))
            .ToList();

        var locationsLookupByLocationType = allLocationsForStage
            .ToLookup(x => x.LocationType);
        
        return locationsLookupByLocationType;
    }

    private IDictionary<string, ChronologicalLocationModel> GetChronologicalLocationsByLocationName(string stageName)
    {
        return _chronologicalLocationsByStage[stageName]
            .ToDictionary(x => x.LocationName);
    }

    private static void WriteStagePageFooter(StreamWriter streamWriter)
    {
        const string footerText = "</div>";
        
        streamWriter.WriteLine(footerText);
    }
}
