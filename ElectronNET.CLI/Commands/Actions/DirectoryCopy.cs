using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DirectoryCopy
    {
        public static void Do(string sourceDirName, string destDirName, bool copySubDirs, List<string> ignoredSubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory already exists, delete it and create a new one.
            if (Directory.Exists(destDirName))
                Directory.Delete(destDirName, true);

            Directory.CreateDirectory(destDirName);


            // Get the files in the directory and copy them to the new location.
            foreach (FileInfo file in dir.GetFiles())
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;

            foreach (DirectoryInfo subdir in dirs.Where(s => !ignoredSubDirs.Contains(s.Name)))
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                Do(subdir.FullName, temppath, copySubDirs, ignoredSubDirs);
            }
        }
    }
}
