using Sa2ApWiki.Common;
using Sa2ApWiki.Common.Services;
using Sa2ApWiki.MirahezePageGeneratorScript;

const string path = ProjectPath.Path;

var chronologicalLocationDocumentPath = Path.Join(path, "ChronologicalLocationOrder.txt");
var chronologicalLocationsByStage = ChronologicalLocationOrderParser.Parse(chronologicalLocationDocumentPath);

//MarkdownGenerator.GenerateMarkdownFiles(path);

var mirahezeGenerator = new MirahezeGenerator(chronologicalLocationsByStage);
mirahezeGenerator.GenerateMirahezeFiles(path);
