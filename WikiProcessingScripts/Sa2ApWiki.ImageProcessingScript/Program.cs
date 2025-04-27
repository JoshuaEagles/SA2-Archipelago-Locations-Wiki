// Note: this script requires cwebp to be available, and will likely only work on Linux.

using System.Diagnostics;
using Sa2ApWiki.Common;

const string path = ProjectPath.Path;

// Aspect Ratio Fix
// foreach (var filePath in Directory.EnumerateFiles(path, "*.webp", SearchOption.AllDirectories))
// {
//     var renamedFile = filePath.Replace(".webp", "old.webp");
//     File.Move(filePath, renamedFile);
//     Console.WriteLine($"Renaming {filePath} to {renamedFile}");
//
//     Console.WriteLine(RunCommandWithBash($" -c \"cwebp -resize 640 480 -noalpha -m 6 -mt '{renamedFile}' -o '{filePath}'\""));
// }

// Compress pngs
foreach (var filePath in Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories))
{
    var outputPath = filePath.Replace(".png", ".webp");

    Console.WriteLine(RunCommandWithBash($" -c \"cwebp -resize 640 480 -noalpha -m 6 -mt -q 70 '{filePath}' -o '{outputPath}'\""));
}

static string RunCommandWithBash(string command)
{
    var psi = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = command,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(psi) ?? throw new Exception("Failed to execute shell command");

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}
