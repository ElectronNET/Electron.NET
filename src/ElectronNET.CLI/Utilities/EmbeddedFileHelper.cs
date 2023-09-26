using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ElectronNET.CLI;

public static class EmbeddedFileHelper
{
    private const string ResourcePath = "ElectronNET.CLI.{0}";

    private static Stream GetTestResourceFileStream(string folderAndFileInProjectPath)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resource = string.Format(ResourcePath, folderAndFileInProjectPath);

        return asm.GetManifestResourceStream(resource);
    }

    private static string ResolveFolderPath(string path, string[] folderNames)
    {
        var segments = path.Split('.').ToList();
        var reorderedSegments = new List<string>();

        foreach (var folder in folderNames)
        {
            var index = segments.IndexOf(folder);
            if (index != -1)
            {
                reorderedSegments.Add(segments[index]);
                segments.RemoveAt(index);
            }
        }

        reorderedSegments.Add(string.Join(".", segments));

        return string.Join("/", reorderedSegments);
    }

    public static void DeployEmbeddedFolder(string targetPath, string rootPath, string[] include, string[] exclude)
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var basePath = string.Format(ResourcePath, rootPath);
            if (exclude.Any(path => resourceName.Contains(path))) continue;
            if (!resourceName.StartsWith(basePath)) continue;

            var relativePath = ResolveFolderPath(resourceName.Substring(basePath.Length + 1), include);

            var outputPath = Path.Combine(targetPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    Console.WriteLine($"Failed to find resource: {resourceName}");
                    continue;
                }

                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }
    }

    public static void DeployEmbeddedFile(string targetPath, string file, string namespacePath = "")
    {
        using (var fileStream = File.Create(Path.Combine(targetPath, file)))
        {
            var streamFromEmbeddedFile = GetTestResourceFileStream("ElectronHost." + namespacePath + file);
            if (streamFromEmbeddedFile == null)
            {
                Console.WriteLine("Error: Couldn't find embedded file: " + file);
            }

            streamFromEmbeddedFile.CopyTo(fileStream);
        }
    }

    public static void DeployEmbeddedFileToTargetFile(string targetPath, string embeddedFile, string targetFile, string namespacePath = "")
    {
        using (var fileStream = File.Create(Path.Combine(targetPath, targetFile)))
        {
            var streamFromEmbeddedFile = GetTestResourceFileStream("ElectronHost." + namespacePath + embeddedFile);
            if (streamFromEmbeddedFile == null)
            {
                Console.WriteLine("Error: Couldn't find embedded file: " + embeddedFile);
            }

            streamFromEmbeddedFile.CopyTo(fileStream);
        }
    }
}