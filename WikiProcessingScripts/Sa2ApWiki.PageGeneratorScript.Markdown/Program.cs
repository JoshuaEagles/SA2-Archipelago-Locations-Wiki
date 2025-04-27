using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Services;
using Sa2ApWiki.MarkdownPageGeneratorScript;

const string path = ProjectPath.Path;

var chronologicalLocationDocumentPath = Path.Join(path, "ChronologicalLocationOrder.txt");
var chronologicalLocationsByStage = ChronologicalLocationOrderParser.Parse(chronologicalLocationDocumentPath);

new MarkdownGenerator(chronologicalLocationsByStage).GenerateMarkdownFiles(path);
