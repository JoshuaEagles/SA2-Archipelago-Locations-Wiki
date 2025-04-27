using System.Text.RegularExpressions;
using Sa2ApWiki.Common;

const string path = ProjectPath.Path;

var outputFilePath = Path.Join(path, "ChronologicalLocationOrder.txt");
using var streamWriter = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write));

var chronologicalFilesPaths = Directory.EnumerateFiles(path, "*Chronological.md", SearchOption.AllDirectories);

var stageNamesInOrderList = Constants.StageNamesInOrder.ToList();
var sortedChronologicalFiles = chronologicalFilesPaths.OrderBy(filePath =>
{
    var fileName = Path.GetFileName(filePath);
    var stageNamePascalCase = fileName.Substring(0, fileName.IndexOf("Chronological", StringComparison.Ordinal));

    return stageNamesInOrderList.IndexOf(stageNamePascalCase);
});

foreach (var filePath in sortedChronologicalFiles)
{
    var fileName = Path.GetFileName(filePath);
    var stageNamePascalCase = fileName.Substring(0, fileName.IndexOf("Chronological", StringComparison.Ordinal));
    
    var fileLines = File.ReadAllLines(filePath);

    var chronologicalLocationNames = fileLines
        .Where(x => x.StartsWith("##"))
        .Select(x => x.Replace("##", ""))
        .Select(x => x.Trim())
        .Select(x => x.Replace(" ", "-"));
    
    streamWriter.WriteLine($"{stageNamePascalCase} {string.Join(", ", chronologicalLocationNames)}");
    Console.WriteLine($"{stageNamePascalCase} {string.Join(" ", chronologicalLocationNames)}");
}
