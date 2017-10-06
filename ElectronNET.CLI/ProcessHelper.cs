using System;
using System.Diagnostics;

namespace ElectronNET.CLI
{
    public class ProcessHelper
    {
        public static void CmdExecute(string command, string workingDirectoryPath, bool output = true, bool waitForExit = true)
        {
            using (Process cmd = new Process())
            {
                cmd.StartInfo.FileName = "cmd.exe";
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
