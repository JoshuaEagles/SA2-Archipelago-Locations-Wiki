// Note: this script requires cwebp to be available, and will likely only work on Linux.

using System.Diagnostics;
using Sa2ApWiki.Common;

const string path = ProjectPath.Path;

foreach (string filePath in Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories))
{
    string outputPath = filePath.Replace(".jpg", ".webp");

    Console.WriteLine(RunCommandWithBash($" -c \"cwebp -resize 640 512 -m 6 -q 70 '{filePath}' -o '{outputPath}'\""));
}

static string RunCommandWithBash(string command)
{
    var psi = new ProcessStartInfo();
    psi.FileName = "/bin/bash";
    psi.Arguments = command;
    psi.RedirectStandardOutput = true;
    psi.UseShellExecute = false;
    psi.CreateNoWindow = true;

    using var process = Process.Start(psi) ?? throw new Exception("Failed to execute shell command");

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}
