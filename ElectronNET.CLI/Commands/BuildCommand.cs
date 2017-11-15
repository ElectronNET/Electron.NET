using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronNET.CLI.Commands.Actions;

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

                var platformInfo = GetTargetPlatformInformation.Do(desiredPlatform);


                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop", desiredPlatform);
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid}...");

                var resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish.");
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);

                var checkForNodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                if (Directory.Exists(checkForNodeModulesDirPath) == false)
                {
                    Console.WriteLine("node_modules missing in: " + checkForNodeModulesDirPath);

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
                }
                else
                {
                    Console.WriteLine("Skip npm install, because node_modules directory exists in: " + checkForNodeModulesDirPath);
                }

                Console.WriteLine("Build Electron Desktop Application...");
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");

                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                // ToDo: Need a solution for --asar support
                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute($"electron-packager . --platform={platformInfo.ElectronPackerPlatform} --arch=x64 --out=\"{buildPath}\" --overwrite", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }


    }
}
