using System;
using System.IO;
using System.Reflection;

namespace ElectronNET.CLI
{
    public static class EmbeddedFileHelper
    {
        private const string ResourcePath = "ElectronNET.CLI.{0}";
        private const string ResourcePath2 = "ElectronNet.CLI.{0}";

        private static Stream GetTestResourceFileStream(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);
            var resource2 = string.Format(ResourcePath2, folderAndFileInProjectPath);

            var stream = asm.GetManifestResourceStream(resource) ?? asm.GetManifestResourceStream(resource2);

            if(stream is null)
            {
                PrintAllResources();

                Console.WriteLine("Was missing resource: {0}", resource);

                return null;
            }
            else
            {
                return stream;
            }
        }

        public static void PrintAllResources()
        {
            var asm = Assembly.GetExecutingAssembly();

            foreach (var n in asm.GetManifestResourceNames())
            {
                Console.WriteLine("Found resource : {0}", n);
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
}