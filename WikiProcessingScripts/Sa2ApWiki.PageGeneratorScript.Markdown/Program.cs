using Sa2ApWiki.Common.Services;
using Sa2ApWiki.MarkdownPageGeneratorScript;

Console.WriteLine("Enter path to root of project:");
var path = Console.ReadLine();

if (string.IsNullOrEmpty(path)) {
	throw new Exception("Invalid path");
}

var chronologicalLocationDocumentPath = Path.Join(path, "ChronologicalLocationOrder.txt");
var chronologicalLocationsByStage = ChronologicalLocationOrderParser.Parse(chronologicalLocationDocumentPath);

MarkdownGenerator.GenerateMarkdownFiles(path);
