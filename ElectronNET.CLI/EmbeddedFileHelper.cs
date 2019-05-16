using System;
using System.IO;
using System.Reflection;

namespace ElectronNET.CLI {

    /// <summary> An embedded file helper. </summary>
    public static class EmbeddedFileHelper {

        private const string ResourcePath = "ElectronNET.CLI.{0}";

        /// <summary> Gets test resource file stream. </summary>
        /// <param name="folderAndFileInProjectPath"> Full pathname of the folder and file in project file. </param>
        /// <returns> The test resource file stream. </returns>
        private static Stream GetTestResourceFileStream(string folderAndFileInProjectPath) {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);
            return asm.GetManifestResourceStream(resource);
        }

        /// <summary> Deploy embedded file. </summary>
        /// <param name="targetPath">    Full pathname of the target file. </param>
        /// <param name="file">          The file. </param>
        /// <param name="namespacePath"> (Optional) Full pathname of the namespace file. </param>
        public static void DeployEmbeddedFile(string targetPath, string file, string namespacePath = "") {
            using (var fileStream = File.Create(Path.Combine(targetPath, file))) {
                var streamFromEmbeddedFile = GetTestResourceFileStream("ElectronHost." + namespacePath + file);
                if (streamFromEmbeddedFile == null) {
                    Console.WriteLine("Error: Couldn't find embedded file: " + file);
                }
                else {
                    streamFromEmbeddedFile.CopyTo(fileStream);
                }
            }
        }
    }
}
