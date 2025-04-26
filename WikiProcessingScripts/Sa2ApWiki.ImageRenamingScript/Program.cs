/*
 * The point of this script was to rename files like goldbeetle-1.webp to goldbeetle-1-1.webp, to keep things consistent and make parsing easier.
 */

using Sa2ApWiki.Common.Models;

// Console.WriteLine("Enter path to root of project:");
// var path = Console.ReadLine();
var path = "/home/entiss/Projects/SA2-Archipelago-Locations-Wiki/";

if (path is null || path == string.Empty) {
	throw new Exception("Invalid path");
}

PerformFileRename(Directory.EnumerateFiles(path, "*.webp", SearchOption.AllDirectories));
PerformFileRename(Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories));
return;

void PerformFileRename(IEnumerable<string> filePaths)
{
	foreach (var filePath in filePaths)
	{
		var locationScreenshot = new LocationScreenshot(filePath);

		if (locationScreenshot.LocationType == "goldbeetle" || locationScreenshot.LocationType == "lostchao" ||
		    locationScreenshot.LocationType == "upgrade")
		{
			var newFileName =
				$"{locationScreenshot.LocationType}-{locationScreenshot.LocationNumber}-{locationScreenshot.ScreenshotNumber}.webp";

			var fileDirectory = Path.GetDirectoryName(filePath);
			Console.WriteLine(
				$"Renaming {fileDirectory}/{locationScreenshot.FileName} to {fileDirectory}/{newFileName}");

			File.Move($"{fileDirectory}/{locationScreenshot.FileName}", $"{fileDirectory}/{newFileName}");
		}
	}
}
