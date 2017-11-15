using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronNET.CLI.Commands.Actions;

namespace ElectronNET.CLI.Commands
{
    public class StartElectronCommand : ICommand
    {
        public const string COMMAND_NAME = "start";
        public const string COMMAND_DESCRIPTION = "Start your ASP.NET Core Application with Electron, without package it as a single exe. Faster for development.";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public StartElectronCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Start Electron Desktop Application...");

                string aspCoreProjectPath = "";

                if (_args.Length > 0)
                {
                    if (Directory.Exists(_args[0]))
                    {
                        aspCoreProjectPath = _args[0];
                    }
                }
                else
                {
                    aspCoreProjectPath = Directory.GetCurrentDirectory();
                }

                string tempPath = Path.Combine(aspCoreProjectPath, "obj", "Host");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                var platformInfo = GetTargetPlatformInformation.Do(string.Empty);

                string tempBinPath = Path.Combine(tempPath, "bin");
                var resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} --output \"{tempBinPath}\"", aspCoreProjectPath);

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
                }
                else
                {
                    Console.WriteLine("Skip npm install, because node_modules directory exists in: " + checkForNodeModulesDirPath);
                }

                string path = Path.Combine(tempPath, "node_modules", ".bin");

                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {
                    Console.WriteLine("Invoke electron.cmd - in dir: " + path);
                    ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js""", path);
                }
                else
                {
                    Console.WriteLine("Invoke electron - in dir: " + path);
                    ProcessHelper.CmdExecute(@"./electron ""../../main.js""", path);
                }

                return true;
            });
        }
    }
}
