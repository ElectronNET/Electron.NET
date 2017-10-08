using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElectronNET.CLI
{
    public class ProcessHelper
    {
        public static void CmdExecute(string command, string workingDirectoryPath, bool output = true, bool waitForExit = true)
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
                    // ToDo: Linux...

                    cmd.StartInfo.FileName = "bash";
                }

                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = workingDirectoryPath;

                cmd.Start();

                cmd.StandardInput.WriteLine(command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                if(waitForExit)
                {
                    cmd.WaitForExit();
                }

                if (output)
                {
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                }
            }
        }
    }
}
