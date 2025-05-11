using System.Collections.Immutable;
using Sa2ApWiki.Common;

const string path = ProjectPath.Path;

var chronologicalLocationOrderLines = File.ReadAllLines(Path.Join(path, "ChronologicalLocationOrder.txt")).Where(x => !string.IsNullOrWhiteSpace(x));

var outputFilePath = Path.Join(path, "ChronologicalLocationOrder.txt");
using var streamWriter = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write));

foreach (var line in chronologicalLocationOrderLines)
{
    var splitBySpaces = line.Replace(",", "").Split(' ');
    
    var stageName = splitBySpaces[0];
    var locationNames = splitBySpaces[1..];
    
    var namesWithZeroPaddedNumbers = locationNames
        .Select(x => x.Trim())
        .Select(x =>
        {
            var splitName = x.Split('-');
            var zeroPaddedLocationNumber = Helpers.ZeroPadNumber(int.Parse(splitName[1]), 2);
            return $"{splitName[0]}-{zeroPaddedLocationNumber}";
        })
        .ToImmutableArray();
    
    streamWriter.WriteLine($"{stageName} {string.Join(' ', namesWithZeroPaddedNumbers)}");
    Console.WriteLine($"{stageName} {string.Join(" ", namesWithZeroPaddedNumbers)}");
}
