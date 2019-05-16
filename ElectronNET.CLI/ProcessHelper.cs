using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ElectronNET.CLI {

    /// <summary> Process helper class. </summary>
    public class ProcessHelper {

        /// <summary> The error RegEx. </summary>
        private static readonly Regex ErrorRegex = new Regex(@"\berror\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary> Command execute. </summary>
        /// <param name="command">              The command. </param>
        /// <param name="workingDirectoryPath"> Pathname of the working directory. </param>
        /// <param name="output">               (Optional) True to output. </param>
        /// <param name="waitForExit">          (Optional) True to wait for exit. </param>
        /// <returns> An int. </returns>
        public static int CmdExecute(string command, string workingDirectoryPath, bool output = true, bool waitForExit = true) {
            using (var cmd = new Process()) {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                cmd.StartInfo.FileName = isWindows ? "cmd.exe" : "bash";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.WorkingDirectory = workingDirectoryPath;

                var returnCode = 0;

                if (output) {
                    cmd.OutputDataReceived += (s, e) => {
                        // (sometimes error messages are only visbile here)
                        // poor mans solution, we just seek for the term 'error'

                        // we can't just use cmd.ExitCode, because
                        // we delegate it to cmd.exe, which runs fine
                        // but we can catch any error here and return
                        // 1 if something fails
                        if (e != null && string.IsNullOrWhiteSpace(e.Data) == false) {
                            if (ErrorRegex.IsMatch(e.Data))
                                returnCode = 1;

                            Console.WriteLine(e.Data);
                        }

                    };
                    cmd.ErrorDataReceived += (s, e) => {
                        // poor mans solution, we just seek for the term 'error'

                        // we can't just use cmd.ExitCode, because
                        // we delegate it to cmd.exe, which runs fine
                        // but we can catch any error here and return
                        // 1 if something fails
                        if (e != null && string.IsNullOrWhiteSpace(e.Data) == false) {
                            if (ErrorRegex.IsMatch(e.Data))
                                returnCode = 1;

                            Console.WriteLine(e.Data);
                        }

                    };
                }

                cmd.Start();
                cmd.BeginOutputReadLine();
                cmd.BeginErrorReadLine();

                cmd.StandardInput.WriteLine(command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                if (waitForExit)
                    cmd.WaitForExit();

                return returnCode;
            }
        }
    }
}
