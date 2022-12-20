using System;
using System.Diagnostics;
using System.IO;

Console.WriteLine("Hello, World!");
// cwebp -resize 640 512 -m 6 -q 70 ../Sonic/CityEscape/Chaobox-1st-Close.webp -o ../Sonic/CityEscape/Chaobox-1st-Close.webp

// Console.WriteLine(RunCommandWithBash(" -c \"cwebp -resize 640 512 -m 6 -q 70 ../Sonic/CityEscape/Chaobox-1st-Close.webp -o ../Sonic/CityEscape/Chaobox-1st-Close.webp\""));

Console.WriteLine("Enter path to root of project:");
string path = Console.ReadLine();

foreach (string filePath in Directory.EnumerateFiles(path, "*.webp", SearchOption.AllDirectories))
{
    string outputPath = filePath.Replace(".webp", ".webp");

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

    using var process = Process.Start(psi);

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}
