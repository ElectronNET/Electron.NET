using System;
using System.Collections.Generic;
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
        public static string COMMAND_ARGUMENTS = "Optional: '/path' from ASP.NET Core Project." + Environment.NewLine +
            "Optional: '/target' with params 'win/osx/linux' to build for a typical app or use 'custom' and specify .NET Core build config & electron build config" + Environment.NewLine +
            " for custom target, check .NET Core RID Catalog and Electron build target/" + Environment.NewLine +
            " e.g. '/target win' or '/target custom \"win7-x86;win32\"'";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption> ();

        private string[] _args;

        public StartElectronCommand(string[] args)
        {
            _args = args;
        }

        private string _paramPath = "path";
        private string _paramTarget = "target";

        public Task<bool> ExecuteAsync () {
            return Task.Run (() => {
                Console.WriteLine ("Start Electron Desktop Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser ();
                parser.Parse (_args);

                var desiredPlatform = string.Empty;
                string specifiedFromCustom = string.Empty;

                if (parser.Arguments.ContainsKey (_paramTarget)) {
                    desiredPlatform = parser.Arguments[_paramTarget][0];
                    if (desiredPlatform == "custom" && parser.Arguments[_paramTarget].Length > 1) {
                        specifiedFromCustom = parser.Arguments[_paramTarget][1];
                    }
                }

                string aspCoreProjectPath = "";

                if (parser.Arguments.ContainsKey (_paramPath)) {
                    string pathTemp = parser.Arguments[_paramPath][0];
                    if (Directory.Exists (pathTemp)) {
                        aspCoreProjectPath = pathTemp;
                    }
                } else {
                    aspCoreProjectPath = Directory.GetCurrentDirectory ();
                }

                string tempPath = Path.Combine(aspCoreProjectPath, "obj", "Host");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                var platformInfo = GetTargetPlatformInformation.Do (desiredPlatform, specifiedFromCustom);

                string tempBinPath = Path.Combine(tempPath, "bin");
                var resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} --output \"{tempBinPath}\"", aspCoreProjectPath);

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);

                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                Console.WriteLine("node_modules missing in: " + nodeModulesDirPath);

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("ElectronHostHook handling started...");

                string electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");

                if (Directory.Exists(electronhosthookDir))
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                    Console.WriteLine("Start npm install for hosthooks...");
                    ProcessHelper.CmdExecute("npm install", hosthookDir);

                    string tscPath = Path.Combine(tempPath, "node_modules", ".bin");
                    // ToDo: Not sure if this runs under linux/macos
                    ProcessHelper.CmdExecute(@"tsc -p ../../ElectronHostHook", tscPath);
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
