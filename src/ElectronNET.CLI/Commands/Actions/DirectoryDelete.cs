using System;
using System.IO;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DirectoryDelete
    {
        public static void Do(string filePath)
        {
            try
            {
                Directory.Delete(filePath, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Attempt to reset directory permissions and try again.
                var di = new DirectoryInfo(filePath);
                di.Attributes &= ~FileAttributes.ReadOnly;
                foreach (var dir in di.GetDirectories())
                {
                    dir.Attributes &= ~FileAttributes.ReadOnly;
                }
                foreach (var file in di.GetFiles())
                {
                    file.Attributes &= ~FileAttributes.ReadOnly;
                }
                Directory.Delete(filePath, true);
            }
        }
    }
}