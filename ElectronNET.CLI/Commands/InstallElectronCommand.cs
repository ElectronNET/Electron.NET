using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    public class InstallElectronCommand : ICommand
    {
        public const string COMMAND_NAME = "install";
        public const string COMMAND_DESCRIPTION = "install";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        public bool DisableInteractive { get; set; }

        private string[] _args;

        public InstallElectronCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Install Electron Host...");

                string currentPath = Directory.GetCurrentDirectory();

                string targetPath = Path.Combine(currentPath, "..", "CLIDeploy");

                Console.WriteLine("Target: " + targetPath);

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }

                DeployEmbeddedFile(targetPath, "main.js");
                DeployEmbeddedFile(targetPath, "package.json");
                DeployEmbeddedFile(targetPath, "package-lock.json");

                Console.WriteLine("Start npm install...");
                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.WorkingDirectory = targetPath;

                    cmd.Start();

                    cmd.StandardInput.WriteLine("npm install");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                }

                return true;
            });
        }

        private static void DeployEmbeddedFile(string targetPath, string file)
        {
            using (var fileStream = File.Create(Path.Combine(targetPath, file)))
            {
                var streamFromEmbeddedFile = EmbeddedFileHelper.GetTestResourceFileStream("ElectronHost." + file);
                streamFromEmbeddedFile.CopyTo(fileStream);
            }
        }
    }
}
