using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    public class BuildCommand : ICommand
    {
        public const string COMMAND_NAME = "build";
        public const string COMMAND_DESCRIPTION = "Build your Electron Application.";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Windows Application...");

                string currentAssemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                string targetPath = Path.Combine(currentAssemblyPath, "Host");

                Console.WriteLine("Target: " + targetPath);

                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

                    cmd.Start();

                    cmd.StandardInput.WriteLine("dotnet publish -r win10-x64 --output " + Path.Combine(targetPath, "bin"));
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                }

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

                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.WorkingDirectory = targetPath;

                    cmd.Start();

                    cmd.StandardInput.WriteLine("npm install electron-packager --global");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                }

                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.WorkingDirectory = targetPath;

                    cmd.Start();

                    string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop"); 
                    cmd.StandardInput.WriteLine($"electron-packager . --platform=win32 --arch=x64 --out=\"{buildPath}\" --overwrite");
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
