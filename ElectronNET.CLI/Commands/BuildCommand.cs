using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
                Console.WriteLine("Build Electron Application...");

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop");

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {
                    Console.WriteLine("Build ASP.NET Core App for Windows...");

                    ProcessHelper.CmdExecute($"dotnet publish -r win10-x64 --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());
                }
                else
                {
                    Console.WriteLine("Build ASP.NET Core App for OSX...");

                    // ToDo: Linux... (or maybe via an argument, but this is just for development)
                    ProcessHelper.CmdExecute($"dotnet publish -r osx-x64 --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());
                }

                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "main.js");
                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
                EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package-lock.json");

                string hostApiFolder = Path.Combine(tempPath, "api");
                if (Directory.Exists(hostApiFolder) == false)
                {
                    Directory.CreateDirectory(hostApiFolder);
                }
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "ipc.js", "api.");

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("Start npm install electron-packager...");

                if (isWindows)
                {
                    ProcessHelper.CmdExecute("npm install electron-packager --global", tempPath);
                }
                else
                {
                    // ToDo: find another solution or document it proper
                    // GH Issue https://github.com/electron-userland/electron-prebuilt/issues/48
                    Console.WriteLine("Electron Packager - make sure you invoke 'sudo npm install electron - packager--global' at " + tempPath + " manually. Sry.");
                }

                Console.WriteLine("Build Electron Desktop Application...");
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");
                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                // ToDo: Need a solution for --asar support
                if (isWindows)
                {
                    Console.WriteLine("Package Electron App for Windows...");

                    ProcessHelper.CmdExecute($"electron-packager . --platform=win32 --arch=x64 --out=\"{buildPath}\" --overwrite", tempPath);
                }
                else
                {
                    Console.WriteLine("Package Electron App for OSX...");

                    // ToDo: Linux... (or maybe via an argument, but this is just for development)
                    ProcessHelper.CmdExecute($"electron-packager . --platform=darwin --arch=x64 --out=\"{buildPath}\" --overwrite", tempPath);
                }

                return true;
            });
        }


    }
}
