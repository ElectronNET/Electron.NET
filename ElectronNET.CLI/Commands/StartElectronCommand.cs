using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly string[] _args;

        public StartElectronCommand(string[] args)
        {
            _args = args;
        }

        private string _aspCoreProjectPath = "project-path";
        private string _paramDotNetProject = "dotnet-project";
        private string _arguments = "args";
        private string _manifest = "manifest";
        private string _clearCache = "clear-cache";
        private string _paramDotNetConfig = "dotnet-configuration";
        private string _paramTarget = "target";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Start Electron Desktop Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser();
                parser.Parse(_args);

                string aspCoreProjectPath = "";

                if (parser.Arguments.ContainsKey(_aspCoreProjectPath))
                {
                    string projectPath = parser.Arguments[_aspCoreProjectPath].First();
                    if (Directory.Exists(projectPath))
                    {
                        aspCoreProjectPath = projectPath;
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

                string tempBinPath = Path.GetFullPath(Path.Combine(tempPath, "bin"));

                var dotNetPublishFlags = BuildCommand.GetDotNetPublishFlags(parser, "false", "false");

                var resultCode = 0;


                // If target is specified as a command line argument, use it.
                // Format is the same as the build command.
                // If target is not specified, autodetect it.
                var platformInfo = GetTargetPlatformInformation.Do(string.Empty, string.Empty);
                if (parser.Arguments.ContainsKey(_paramTarget))
                {
                    var desiredPlatform = parser.Arguments[_paramTarget][0];
                    string specifiedFromCustom = string.Empty;
                    if (desiredPlatform == "custom" && parser.Arguments[_paramTarget].Length > 1)
                    {
                        specifiedFromCustom = parser.Arguments[_paramTarget][1];
                    }
                    platformInfo = GetTargetPlatformInformation.Do(desiredPlatform, specifiedFromCustom);
                }

                string configuration = "Debug";
                if (parser.Arguments.ContainsKey(_paramDotNetConfig))
                {
                    configuration = parser.Arguments[_paramDotNetConfig][0];
                }

                var project = string.Empty;
                if (parser.Arguments.ContainsKey(_paramDotNetProject))
                {
                    project = parser.Arguments[_paramDotNetProject][0];
                }

                if (!parser.Arguments.ContainsKey("watch"))
                {
                    resultCode = ProcessHelper.CmdExecute($"dotnet publish {project} -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {string.Join(' ', dotNetPublishFlags.Select(kvp => $"{kvp.Key}={kvp.Value}"))} /p:DisabledWarnings=true", aspCoreProjectPath);
                }

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);

                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                bool runNpmInstall = false;

                Console.WriteLine("node_modules in: " + nodeModulesDirPath);

                if (!Directory.Exists(nodeModulesDirPath))
                {
                    runNpmInstall = true;
                }

                var packagesJson = Path.Combine(tempPath, "package.json");
                var packagesPrevious = Path.Combine(tempPath, "package.json.previous");

                if (!runNpmInstall)
                {

                    if (File.Exists(packagesPrevious))
                    {
                        if (File.ReadAllText(packagesPrevious) != File.ReadAllText(packagesJson))
                        {
                            runNpmInstall = true;
                        }
                    }
                    else
                    {
                        runNpmInstall = true;
                    }
                }

                if (runNpmInstall)
                {
                    Console.WriteLine("Start npm install...");
                    ProcessHelper.CmdExecute("npm install", tempPath);
                    File.Copy(packagesJson, packagesPrevious, true);
                }

                Console.WriteLine("ElectronHostHook handling started...");

                string electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");

                if (Directory.Exists(electronhosthookDir))
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                    Console.WriteLine("Start npm install for typescript & hosthooks...");
                    ProcessHelper.CmdExecute("npm install", hosthookDir);

                    // ToDo: Not sure if this runs under linux/macos
                    ProcessHelper.CmdExecute(@"npx tsc -p ../../ElectronHostHook", tempPath);
                }

                string arguments = "";

                if (parser.Arguments.ContainsKey(_arguments))
                {
                    arguments = string.Join(' ', parser.Arguments[_arguments]);
                }

                if (parser.Arguments.ContainsKey(_manifest))
                {
                    arguments += " --manifest=" + parser.Arguments[_manifest].First();
                }

                if (parser.Arguments.ContainsKey(_clearCache))
                {
                    arguments += " --clear-cache=true";
                }

                if (parser.Arguments.ContainsKey("watch"))
                {
                    arguments += " --watch=true";
                }

                string path = Path.Combine(tempPath, "node_modules", ".bin");
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                if (isWindows)
                {
                    Console.WriteLine("Invoke electron.cmd - in dir: " + path);
                    Console.WriteLine("\n\n---------------------------------------------------\n\n\n");
                    ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js"" " + arguments, path);

                }
                else
                {
                    Console.WriteLine("Invoke electron - in dir: " + path);
                    Console.WriteLine("\n\n---------------------------------------------------\n\n\n");
                    ProcessHelper.CmdExecute(@"./electron ""../../main.js"" " + arguments, path);
                }

                return true;
            });
        }
    }
}
