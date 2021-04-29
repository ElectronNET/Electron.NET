using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElectronNET.CLI
{
    public class ProcessHelper
    {
        public static int CmdExecute(string command, string workingDirectoryPath, bool output = true, bool waitForExit = true)
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

                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = workingDirectoryPath;

                if (output)
                {
                    cmd.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                    cmd.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
                }

                Console.WriteLine(command);
                cmd.Start();
                cmd.BeginOutputReadLine();
                cmd.BeginErrorReadLine();

                if (waitForExit)
                {
                    cmd.WaitForExit();
                }

                return cmd.ExitCode;
            }
        }
    }
}
