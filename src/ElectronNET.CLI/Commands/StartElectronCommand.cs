﻿using System;
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

        private string[] _args;

        public StartElectronCommand(string[] args)
        {
            _args = args;
        }

        private string _aspCoreProjectPath = "project-path";
        private string _arguments = "args";
        private string _manifest = "manifest";
        private string _clearCache = "clear-cache";
        private string _paramPublishReadyToRun = "PublishReadyToRun";
        private string _paramPublishSingleFile = "PublishSingleFile";
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
                    Directory.CreateDirectory(tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");
                var resultCode = 0;

                string publishReadyToRun = "/p:PublishReadyToRun=";
                if (parser.Arguments.ContainsKey(_paramPublishReadyToRun))
                {
                    publishReadyToRun += parser.Arguments[_paramPublishReadyToRun][0];
                }
                else
                {
                    publishReadyToRun += "true";
                }

                string publishSingleFile = "/p:PublishSingleFile=";
                if (parser.Arguments.ContainsKey(_paramPublishSingleFile))
                {
                    publishSingleFile += parser.Arguments[_paramPublishSingleFile][0];
                }
                else
                {
                    publishSingleFile += "true";
                }

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

                if (parser != null && !parser.Arguments.ContainsKey("watch"))
                {
                    resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {publishReadyToRun} {publishSingleFile} --no-self-contained", aspCoreProjectPath);
                }

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                Console.WriteLine("ElectronHostHook handling started...");

                var hostHookFolders = Directory.GetDirectories(aspCoreProjectPath, "ElectronHostHook", SearchOption.AllDirectories);

                DeployEmbeddedElectronFiles.Do(tempPath, hostHookFolders.Length > 0);

                if (hostHookFolders.Length > 0)
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(hostHookFolders.First(), hosthookDir, true, new List<string>() { "node_modules" });
                }

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

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
                    ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js"" " + arguments, path);

                }
                else
                {
                    Console.WriteLine("Invoke electron - in dir: " + path);
                    ProcessHelper.CmdExecute(@"./electron ""../../main.js"" " + arguments, path);
                }

                return true;
            });
        }
    }
}
