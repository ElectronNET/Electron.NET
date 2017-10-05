using System;
using System.IO;
using System.Reflection;

namespace ElectronNET.CLI
{
    public static class EmbeddedFileHelper
    {
        private const string ResourcePath = "ElectronNET.CLI.{0}";

        private static Stream GetTestResourceFileStream(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);

            return asm.GetManifestResourceStream(resource);
        }
        public static void DeployEmbeddedFile(string targetPath, string file)
        {
            using (var fileStream = File.Create(Path.Combine(targetPath, file)))
            {
                var streamFromEmbeddedFile = GetTestResourceFileStream("ElectronHost." + file);
                streamFromEmbeddedFile.CopyTo(fileStream);
            }
        }
    }
}