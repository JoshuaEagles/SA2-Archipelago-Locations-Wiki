using System.Collections.Immutable;

namespace ChronologicalDataCollectionScript;

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

                var newFilePath = $"{directoryPath}Miraheze.txt";
                using var streamWriter = new StreamWriter(new FileStream(newFilePath, FileMode.Create, FileAccess.Write));

                var stageName = Path.GetFileName(directoryPath);
                
                // TODO: add green hill and co to chronological page
                if (stageName == "GreenHill" || stageName == "Route101" || stageName == "Route280")
                {
                    continue;
                }
            
                WriteStagePageHeader(stageName, streamWriter);
                WriteAllLocationToStagePage(directoryPath, streamWriter, stageName);;
                WriteStagePageFooter(streamWriter);
            }
        }

    }

    private static void WriteStagePageHeader(string stageName, StreamWriter streamWriter)
    {
        // TODO: header needs the chronological first location
        var readableStageName = Constants.StageCodeNameToReadableName[stageName];
        var headerText = $$$"""
                         __NOTOC__{{DISPLAYTITLE:{{{readableStageName}}} Locations}}
                         Display as Chronological:
                         <div id="chronological-toggle">button</div>
                         
                         <div id="chronological-first-location" data-chronological-first-location="pipe-1"></div>

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
                    chronologicalLocationsByLocationName[currentLocationName].NextLocation?.FirstLocationName;
                
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
                
                streamWriter.WriteLine($"=== {locationType.ReadableName} {locationGroup.Key} ===");
                
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
        var allLocationsForStage = new List<LocationScreenshot>();
        foreach (var screenshotPath in Directory.EnumerateFiles(directoryPath, $"*", SearchOption.TopDirectoryOnly))
        {
            var screenshotNameWithoutExtension = Path.GetFileNameWithoutExtension(screenshotPath);
            var screenshotNameWithExtension = Path.GetFileName(screenshotPath);

            var screenshotSplit = screenshotNameWithoutExtension.Split('-');
            
            // TODO: handle bonus properly
            if (screenshotSplit[1].Contains("bonus"))
            {
                continue;
            }

            // TODO: make gold beetles follow the naming convention of type-locationnumber-screenshotnumber
            var itemType = screenshotSplit[0];
            var itemNumber = 1;
            int screenshotNumber;
            if (itemType == "goldbeetle" || itemType == "lostchao" || itemType == "upgrade")
            {
                screenshotNumber = int.Parse(screenshotSplit[1]);
            }
            else 
            {
                itemNumber = int.Parse(screenshotSplit[1]);
                screenshotNumber = int.Parse(screenshotSplit[2]);
            }
                
            allLocationsForStage.Add(new LocationScreenshot
            {
                LocationName = $"{itemType}-{itemNumber}",
                FileName = screenshotNameWithExtension, 
                LocationType = itemType, 
                LocationNumber = itemNumber, 
                ScreenshotNumber = screenshotNumber
            });
        }
        
        var locationsLookupByLocationType = allLocationsForStage
            .ToLookup(x => x.LocationType);
        return locationsLookupByLocationType;
    }

    private IDictionary<string, ChronologicalLocationModel> GetChronologicalLocationsByLocationName(string stageName)
    {
        // TODO: remove the filtering here
        var chronologicalLocations = _chronologicalLocationsByStage[stageName]
            .ToImmutableArray();
        
        var chronologicalLocationsByFirstLocationName = chronologicalLocations
            .ToDictionary(x => x.FirstLocationName);
        var chronologicalLocationsBySecondLocationName = chronologicalLocations
            .Where(x => x.SecondLocationName is not null)
            .ToDictionary(x => x.SecondLocationName!);
        
        var chronologicalLocationsByLocationName = chronologicalLocationsByFirstLocationName
            .Concat(chronologicalLocationsBySecondLocationName)
            .ToDictionary();
        
        return chronologicalLocationsByLocationName;
    }

    private static void WriteStagePageFooter(StreamWriter streamWriter)
    {
        const string footerText = "</div>";
        
        streamWriter.WriteLine(footerText);
    }

    record LocationScreenshot
    {
        public required string LocationName { get; init; }
        public required string FileName { get; init; }
        public required string LocationType { get; init; }
        public required int LocationNumber { get; init; }
        public required int ScreenshotNumber { get; init; }
    }

    /*
    var outputFilePath = Path.Join(path, "ChronologicalLocationOrder.txt");
    using var streamWriter = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write));

    foreach (var filePath in Directory.EnumerateFiles(path, "*Chronological.md", SearchOption.AllDirectories))
    {
        var fileName = Path.GetFileName(filePath);
        var stageNamePascalCase = fileName.Substring(0, fileName.IndexOf("Chronological", StringComparison.Ordinal));
        
        var fileLines = File.ReadAllLines(filePath);
        
        var stageNamePrintedFormat = fileLines.First().Trim().Substring(2, fileLines.First().Trim().IndexOf('(') - 3);
        
        var chronologicalLocationStrings = fileLines
            .Where(x => x.StartsWith("##"))
            .Select(x => x.Replace(stageNamePrintedFormat, "")[2..])
            .Select(x => x.Trim())
            .Select(x =>
            {
                if (x.Contains("Gold Beetle"))
                {
                    return new LocationModel { FirstLocationName = "goldbeetle" };
                }
                
                if (x.Contains('&'))
                {
                    var ampersandIndex = x.IndexOf('&');
                    
                    var firstLocationNumber = Regex.Match(x, "\\d+").Value;
                    var firstLocationType = x.Substring(0, x.IndexOf(firstLocationNumber, StringComparison.Ordinal) - 1);
                    
                    var secondLocationString = x[(ampersandIndex + 2)..];
                    
                    var secondLocationNumber = Regex.Match(secondLocationString, "\\d+").Value;
                    var secondLocationType = secondLocationString[..(secondLocationString.IndexOf(secondLocationNumber, StringComparison.Ordinal) - 1)];
                    
                    return new LocationModel()
                    {
                        FirstLocationName = $"{firstLocationType.ToLower()}-{firstLocationNumber}",
                        SecondLocationName = $"{secondLocationType.ToLower()}-{secondLocationNumber}",
                    };
                }
                
                var locationNumber = Regex.Match(x, "\\d+").Value;
                var locationType = x[..(x.IndexOf(locationNumber, StringComparison.Ordinal) - 1)];
                
                return new LocationModel {FirstLocationName = $"{locationType.ToLower()}-{locationNumber}"};
            })
            .Select(x =>
            {
                if (x.SecondLocationName is not null)
                {
                    return $"{x.FirstLocationName} + {x.SecondLocationName}";
                }
                
                return x.FirstLocationName;
            });
        
        streamWriter.WriteLine($"{stageNamePascalCase} {string.Join(", ", chronologicalLocationStrings)}");
    }

    record LocationModel
    {
        public required string FirstLocationName { get; init; }
        public string? SecondLocationName { get; init; }
    }
    */
}
