using System;
using System.Collections.Generic;
using System.IO;
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

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop");

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");
                ProcessHelper.CmdExecute($"dotnet publish -r win10-x64 --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());

                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "main.js");
                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package-lock.json");

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("Start npm install electron-packager...");
                ProcessHelper.CmdExecute("npm install electron-packager --global", tempPath);

                Console.WriteLine("Build Electron Desktop Application...");
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");
                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                // Need a solution for --asar support
                ProcessHelper.CmdExecute($"electron-packager . --platform=win32 --arch=x64 --electronVersion=1.7.8 --out=\"{buildPath}\" --overwrite", tempPath);

                return true;
            });
        }


    }
}
