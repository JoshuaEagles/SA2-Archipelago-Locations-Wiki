// Note: this script requires cwebp to be available, and will likely only work on Linux.

using System.Diagnostics;
using System.Text.RegularExpressions;

Console.WriteLine("Enter path to root of project:");
var path = Console.ReadLine();

if (string.IsNullOrEmpty(path)) {
    throw new Exception("Invalid path");
}

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
