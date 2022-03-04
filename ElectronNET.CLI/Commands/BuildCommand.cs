﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.CLI.Commands.Actions;

namespace ElectronNET.CLI.Commands
{
    public class BuildCommand : ICommand
    {
        public const string COMMAND_NAME = "build";
        public const string COMMAND_DESCRIPTION = "Build your Electron Application.";
        public const string COMMAND_ARGUMENTS = 
@"Needed: '/target' with params 'win/osx/linux' to build for a typical app or use 'custom' and specify .NET Core build config & electron build config
for custom target, check .NET Core RID Catalog and Electron build target/
e.g. '/target win' or '/target custom ""win7-x86;win""'
Optional: '/dotnet-configuration' with the desired .NET Core build config e.g. release or debug. Default = Release
Optional: '/electron-arch' to specify the resulting electron processor architecture (e.g. ia86 for x86 builds). Be aware to use the '/target custom' param as well!
Optional: '/electron-params' specify any other valid parameter, which will be routed to the electron-packager.
Optional: '/relative-path' to specify output a subdirectory for output.
Optional: '/absolute-path to specify and absolute path for output.
Optional: '/package-json' to specify a custom package.json file.
Optional: '/install-modules' to force node module install. Implied by '/package-json'
Optional: '/Version' to specify the version that should be applied to both the `dotnet publish` and `electron-builder` commands. Implied by '/Version'
Optional: '/p:[property]' or '/property:[property]' to pass in dotnet publish properties.  Example: '/property:Version=1.0.0' to override the FileVersion
Full example for a 32bit debug build with electron prune: build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params ""--prune=true """;

        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private readonly string[] _args;

        public BuildCommand(string[] args)
        {
            _args = args;
        }

        private const string _defaultElectronVersion = "17.0.1";

        private const string _paramTarget = "target";
        private const string _paramDotNetConfig = "dotnet-configuration";
        private const string _paramElectronArch = "electron-arch";
        private const string _paramElectronParams = "electron-params";
        private const string _paramElectronVersion = "electron-version";
        private const string _paramOutputDirectory = "relative-path";
        private const string _paramAbsoluteOutput = "absolute-path";
        private const string _paramPackageJson = "package-json";
        private const string _paramForceNodeInstall = "install-modules";
        private const string _manifest = "manifest";
        private const string _paramPublishReadyToRun = "PublishReadyToRun";
        private const string _paramPublishSingleFile = "PublishSingleFile";
        private const string _paramVersion = "Version";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Electron Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser();
                parser.Parse(_args);

                //This version will be shared between the dotnet publish and electron-builder commands
                string version = null;
                if (parser.Arguments.ContainsKey(_paramVersion))
                    version = parser.Arguments[_paramVersion][0];

                if (!parser.Arguments.ContainsKey(_paramTarget))
                {
                    Console.WriteLine($"Error: missing '{_paramTarget}' argument.");
                    Console.WriteLine(COMMAND_ARGUMENTS);
                    return false;
                }

                var desiredPlatform = parser.Arguments[_paramTarget][0];
                string specifiedFromCustom = string.Empty;
                if (desiredPlatform == "custom" && parser.Arguments[_paramTarget].Length > 1)
                {
                    specifiedFromCustom = parser.Arguments[_paramTarget][1];
                }

                string configuration = "Release";
                if (parser.Arguments.ContainsKey(_paramDotNetConfig))
                {
                    configuration = parser.Arguments[_paramDotNetConfig][0];
                }

                var platformInfo = GetTargetPlatformInformation.Do(desiredPlatform, specifiedFromCustom);

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid}...");

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop", desiredPlatform);

                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }
                else
                {
                    Directory.Delete(tempPath, true);
                    Directory.CreateDirectory(tempPath);
                }


                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid} under {configuration}-Configuration...");
                
                var dotNetPublishFlags = GetDotNetPublishFlags(parser, "false", "false");

                var command =
                    $"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {string.Join(' ', dotNetPublishFlags.Select(kvp => $"{kvp.Key}={kvp.Value}"))} --self-contained";
                
                // output the command 
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(command);
                Console.ResetColor();

                var resultCode = ProcessHelper.CmdExecute(command, Directory.GetCurrentDirectory());

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);
                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                if (parser.Arguments.ContainsKey(_paramPackageJson))
                {
                    Console.WriteLine("Copying custom package.json.");

                    File.Copy(parser.Arguments[_paramPackageJson][0], Path.Combine(tempPath, "package.json"), true);
                }

                var checkForNodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                if (!Directory.Exists(checkForNodeModulesDirPath)|| parser.Contains(_paramForceNodeInstall) || parser.Contains(_paramPackageJson))
                {
                    Console.WriteLine("Start npm install...");
                    ProcessHelper.CmdExecute("npm install --production", tempPath);
                }

                Console.WriteLine("ElectronHostHook handling started...");

                string electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");

                if (Directory.Exists(electronhosthookDir))
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                    Console.WriteLine("Start npm install for hosthooks...");
                    ProcessHelper.CmdExecute("npm install", hosthookDir);

                    // ToDo: Not sure if this runs under linux/macos
                    ProcessHelper.CmdExecute(@"npx tsc -p . --sourceMap false", hosthookDir);
                }

                Console.WriteLine("Build Electron Desktop Application...");

                // Specifying an absolute path supercedes a relative path
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");
                if (parser.Arguments.ContainsKey(_paramAbsoluteOutput))
                {
                    buildPath = parser.Arguments[_paramAbsoluteOutput][0];
                }
                else if (parser.Arguments.ContainsKey(_paramOutputDirectory))
                {
                    buildPath = Path.Combine(Directory.GetCurrentDirectory(), parser.Arguments[_paramOutputDirectory][0]);
                }

                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                string electronArch = "x64";
                if (parser.Arguments.ContainsKey(_paramElectronArch))
                {
                    electronArch = parser.Arguments[_paramElectronArch][0];
                }

                var electronVersion = _defaultElectronVersion;
                if (parser.Arguments.ContainsKey(_paramElectronVersion))
                {
                    electronVersion = parser.Arguments[_paramElectronVersion][0];
                }

                string electronParams = "";
                if (parser.Arguments.ContainsKey(_paramElectronParams))
                {
                    electronParams = parser.Arguments[_paramElectronParams][0];
                }

                // ToDo: Make the same thing easer with native c# - we can save a tmp file in production code :)
                Console.WriteLine("Create electron-builder configuration file...");

                string manifestFileName = "electron.manifest.json";

                if (parser.Arguments.ContainsKey(_manifest))
                {
                    manifestFileName = parser.Arguments[_manifest].First();
                }

                ProcessHelper.CmdExecute(
                    string.IsNullOrWhiteSpace(version)
                        ? $"node build-helper.js {manifestFileName}"
                        : $"node build-helper.js {manifestFileName} {version}", tempPath);

                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute($"npx electron-builder --config=./bin/electron-builder.json --{platformInfo.ElectronPackerPlatform} --{electronArch} -c.electronVersion={electronVersion} {electronParams}", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }

        internal static Dictionary<string, string> GetDotNetPublishFlags(SimpleCommandLineParser parser, string defaultReadyToRun, string defaultSingleFile)
        {
            var dotNetPublishFlags = new Dictionary<string, string>
            {
                {"/p:PublishReadyToRun", parser.TryGet(_paramPublishReadyToRun, out var rtr) ? rtr[0] : defaultReadyToRun},
                {"/p:PublishSingleFile", parser.TryGet(_paramPublishSingleFile, out var psf) ? psf[0] : defaultSingleFile},
            };

            if (parser.Arguments.ContainsKey(_paramVersion))
            {
                if(parser.Arguments.Keys.All(key => !key.StartsWith("p:Version=") && !key.StartsWith("property:Version=")))
                    dotNetPublishFlags.Add("/p:Version", parser.Arguments[_paramVersion][0]);
                if(parser.Arguments.Keys.All(key => !key.StartsWith("p:ProductVersion=") && !key.StartsWith("property:ProductVersion=")))
                    dotNetPublishFlags.Add("/p:ProductVersion", parser.Arguments[_paramVersion][0]);
            }

            foreach (var parm in parser.Arguments.Keys.Where(key => key.StartsWith("p:") || key.StartsWith("property:")))
            {
                var split = parm.IndexOf('=');
                if (split < 0)
                {
                    continue;
                }

                var key = $"/{parm.Substring(0, split)}";
                // normalize the key
                if (key.StartsWith("/property:"))
                {
                    key = key.Replace("/property:", "/p:");
                }

                var value = parm.Substring(split + 1);

                if (dotNetPublishFlags.ContainsKey(key))
                {
                    dotNetPublishFlags[key] = value;
                }
                else
                {
                    dotNetPublishFlags.Add(key, value);
                }
            }

            return dotNetPublishFlags;
        }
    }
}