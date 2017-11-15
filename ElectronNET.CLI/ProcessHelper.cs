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
                    cmd.StartInfo.FileName = "cmd.exe";
                }
                else
                {
                    // works for OSX and Linux (at least on Ubuntu)
                    cmd.StartInfo.FileName = "bash";
                }

                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = workingDirectoryPath;

                int returnCode = 0;

                if (output)
                {
                    cmd.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                    cmd.ErrorDataReceived += (s, e) =>
                    {
                        // we can't just use cmd.ExitCode, because
                        // we delegate it to cmd.exe, which runs fine
                        // but we can catch any error here and return
                        // 1 if something fails
                        returnCode = 1;
                        Console.WriteLine(e.Data);
                    };
                }

                cmd.Start();
                cmd.BeginOutputReadLine();
                cmd.BeginErrorReadLine();

                cmd.StandardInput.WriteLine(command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                if (waitForExit)
                {
                    cmd.WaitForExit();
                }

                return returnCode;
            }
        }
    }
}
