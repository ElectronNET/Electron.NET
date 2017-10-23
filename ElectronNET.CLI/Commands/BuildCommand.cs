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
        public const string COMMAND_ARGUMENTS = "<Platform> to build (default is current OS, possible values are: win,osx,linux)";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public BuildCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Electron Application...");

                string desiredPlatform = "";

                if (_args.Length > 0)
                {
                    desiredPlatform = _args[0];
                }


                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop");

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                string netCorePublishRid = string.Empty;
                string electronPackerPlatform = string.Empty;

                switch (desiredPlatform)
                {
                    case "win":
                        netCorePublishRid = "win10-x64";
                        electronPackerPlatform = "win32";
                        break;
                    case "osx":
                        netCorePublishRid = "osx-x64";
                        electronPackerPlatform = "darwin";
                        break;
                    case "linux":
                        netCorePublishRid = "ubuntu-x64";
                        electronPackerPlatform = "linux";
                        break;
                    default:
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            netCorePublishRid = "win10-x64";
                            electronPackerPlatform = "win32";
                        }
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            netCorePublishRid = "osx-x64";
                            electronPackerPlatform = "darwin";
                        }
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            netCorePublishRid = "ubuntu-x64";
                            electronPackerPlatform = "linux";
                        }

                        break;
                }

                Console.WriteLine($"Build ASP.NET Core App for {netCorePublishRid}...");

                ProcessHelper.CmdExecute($"dotnet publish -r {netCorePublishRid} --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());


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
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "app.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "browserWindows.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "dialog.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "menu.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "notification.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "tray.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "webContents.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "globalShortcut.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "shell.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "screen.js", "api.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "clipboard.js", "api.");

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("Start npm install electron-packager...");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Works proper on Windows...
                    ProcessHelper.CmdExecute("npm install electron-packager --global", tempPath);
                }
                else
                {
                    // ToDo: find another solution or document it proper
                    // GH Issue https://github.com/electron-userland/electron-prebuilt/issues/48
                    Console.WriteLine("Electron Packager - make sure you invoke 'sudo npm install electron-packager --global' at " + tempPath + " manually. Sry.");
                }

                Console.WriteLine("Build Electron Desktop Application...");
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");
                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                // ToDo: Need a solution for --asar support
                Console.WriteLine($"Package Electron App for Platform {electronPackerPlatform}...");
                ProcessHelper.CmdExecute($"electron-packager . --platform={electronPackerPlatform} --arch=x64 --out=\"{buildPath}\" --overwrite", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }


    }
}
