using System.Collections.Generic;
using System.IO;

namespace ElectronNET.CLI.Commands.Actions {

    /// <summary> A directory copy helper. </summary>
    public static class DirectoryCopy {

        /// <summary> Handles copying of recursive directories. </summary>
        /// <exception cref="DirectoryNotFoundException"> Thrown when the requested directory is not
        ///                                               present. </exception>
        /// <param name="sourceDirName">  Pathname of the source directory. </param>
        /// <param name="destDirName">    Pathname of the destination directory. </param>
        /// <param name="copySubDirs">    True to copy sub dirs. </param>
        /// <param name="ignoredSubDirs"> The ignored sub dirs. </param>
        public static void Do(string sourceDirName, string destDirName, bool copySubDirs, List<string> ignoredSubDirs) {

            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }
            else {
                var targetDir = new DirectoryInfo(destDirName);
                foreach (var fileDel in targetDir.EnumerateFiles()) {
                    fileDel.Delete();
                }
                foreach (var dirDel in targetDir.EnumerateDirectories()) {
                    dirDel.Delete(true);
                }
            }


            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (var file in files) {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (var subdir in dirs) {
                    if (ignoredSubDirs.Contains(subdir.Name)) {
                        continue;
                    }

                    var temppath = Path.Combine(destDirName, subdir.Name);
                    Do(subdir.FullName, temppath, copySubDirs, ignoredSubDirs);
                }
            }
        }
    }
}
