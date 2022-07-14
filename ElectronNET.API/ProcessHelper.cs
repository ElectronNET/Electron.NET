using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElectronNET.API
{
    internal class ProcessHelper
    {
        public static void Execute(string command, string workingDirectoryPath)
        {
            using (Process cmd = new Process())
            {
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                if (isWindows)
                {
                    cmd.StartInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                }
                else
                {
                    // works for OSX and Linux (at least on Ubuntu)
                    var escapedArgs = command.Replace("\"", "\\\"");
                    cmd.StartInfo = new ProcessStartInfo("bash", $"-c \"{escapedArgs}\"");
                }

                cmd.StartInfo.RedirectStandardInput = false;
                cmd.StartInfo.RedirectStandardOutput = false;
                cmd.StartInfo.RedirectStandardError = false;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = workingDirectoryPath;
                cmd.Start();
            }
        }
    }
}
