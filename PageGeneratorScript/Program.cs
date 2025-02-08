using System.Text.RegularExpressions;
using System.Linq;

Console.WriteLine("Enter path to root of project:");
var path = Console.ReadLine();

if (string.IsNullOrEmpty(path)) {
    throw new Exception("Invalid path");
}

string[] characterNames = [
    "Sonic",
    "Tails",
    "Knuckles",
    "Rouge",
    "Eggman",
    "Shadow",
    "CannonsCore"
];

foreach (var characterName in characterNames)
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

        WriteLocationsOfSingleType("Item", "item", directoryPath, streamWriter, stageName);
        WriteLocationsOfSingleType("Life", "life", directoryPath, streamWriter, stageName);
    }
}

void WriteLocationsOfSingleType(string readableLocationTypeName, string locationShortName, string directoryPath, StreamWriter streamWriter, string stageName)
{
    var locations = new List<LocationScreenshot>();
    foreach (var screenshotPath in Directory.EnumerateFiles(directoryPath, $"{locationShortName}*", SearchOption.TopDirectoryOnly))
    {
        var screenshotNameWithoutExtension = Path.GetFileNameWithoutExtension(screenshotPath);
        var screenshotNameWithExtension = Path.GetFileName(screenshotPath);

        var screenshotSplit = screenshotNameWithoutExtension.Split('-');

        var itemType = screenshotSplit[0];
        var itemNumber = int.Parse(screenshotSplit[1]);
        var screenshotNumber = int.Parse(screenshotSplit[2]);
            
        locations.Add(new LocationScreenshot
        {
            FileName = screenshotNameWithExtension, 
            ItemType = itemType, 
            ItemNumber = itemNumber, 
            ScreenshotNumber = screenshotNumber
        });
    }
        
    var locationsByItemNumber = locations
        .GroupBy(l => l.ItemNumber)
        .OrderBy(l => l.Key);

    foreach (var locationGroup in locationsByItemNumber)
    {
        streamWriter.WriteLine($"## {stageName} {readableLocationTypeName} {locationGroup.Key}");

        foreach (var location in locationGroup)
        {
            streamWriter.WriteLine($"![](./{stageName}/{location.FileName})");
        }
            
        streamWriter.WriteLine();
        streamWriter.WriteLine("[Back to Top](#)");
        streamWriter.WriteLine();
    }
}

record LocationScreenshot
{
    public required string FileName { get; init; }
    public required string ItemType { get; init; }
    public required int ItemNumber { get; init; }
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