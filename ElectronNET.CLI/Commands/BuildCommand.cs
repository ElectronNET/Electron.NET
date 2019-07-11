﻿using System;
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
        public static string COMMAND_ARGUMENTS = "Needed: '/target' with params 'win/osx/linux' to build for a typical app or use 'custom' and specify .NET Core build config & electron build config" + Environment.NewLine +
                                                 " for custom target, check .NET Core RID Catalog and Electron build target/" + Environment.NewLine +
                                                 " e.g. '/target win' or '/target custom \"win7-x86;win32\"'" + Environment.NewLine +
                                                 "Optional: '/dotnet-configuration' with the desired .NET Core build config e.g. release or debug. Default = Release" + Environment.NewLine +
                                                 "Optional: '/electron-arch' to specify the resulting electron processor architecture (e.g. ia86 for x86 builds). Be aware to use the '/target custom' param as well!" + Environment.NewLine +
                                                 "Optional: '/electron-params' specify any other valid parameter, which will be routed to the electron-packager." + Environment.NewLine +
                                                 "Optional: '/relative-path' to specify output a subdirectory for output." + Environment.NewLine +
                                                 "Optional: '/absolute-path to specify and absolute path for output." + Environment.NewLine +
                                                 "Optional: '/package-json' to specify a custom package.json file." + Environment.NewLine +
                                                 "Optional: '/install-modules' to force node module install. Implied by '/package-json'"  + Environment.NewLine +                                 
                                                 "Full example for a 32bit debug build with electron prune: build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params \"--prune=true \"";

        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public BuildCommand(string[] args)
        {
            _args = args;
        }

        private string _paramTarget = "target";
        private string _paramDotNetConfig = "dotnet-configuration";
        private string _paramElectronArch = "electron-arch";
        private string _paramElectronParams = "electron-params";
        private string _paramOutputDirectory = "relative-path";
        private string _paramAbsoluteOutput = "absolute-path";
        private string _paramPackageJson = "package-json";
        private string _paramForceNodeInstall = "install-modules";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Electron Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser();
                parser.Parse(_args);

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

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid} under {configuration}-Configuration...");

                var resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} -c {configuration} --output \"{tempBinPath}\"", Directory.GetCurrentDirectory());

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);
                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");
                var packageJsonPath = Path.Combine(tempPath, "package.json");

                if (parser.Arguments.ContainsKey(_paramPackageJson))
                {
                    Console.WriteLine("Copying custom package.json.");

                    File.Copy(parser.Arguments[_paramPackageJson][0], packageJsonPath, true);
                }

                var checkForNodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                if (Directory.Exists(checkForNodeModulesDirPath) == false || parser.Contains(_paramForceNodeInstall) || parser.Contains(_paramPackageJson))

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install --production", tempPath);

                Console.WriteLine("Start npm install electron-builder...");
                ProcessHelper.CmdExecute("npm install electron-builder@20.44.4 --save-dev", tempPath);

                Console.WriteLine("ElectronHostHook handling started...");

                string electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");

                if (Directory.Exists(electronhosthookDir))
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                    Console.WriteLine("Start npm install for hosthooks...");
                    ProcessHelper.CmdExecute("npm install --production", hosthookDir);

                    // ToDo: Global TypeScript installation is needed for ElectronHostHook
                    //string tscPath = Path.Combine(tempPath, "node_modules", ".bin");
                    
                    // ToDo: Not sure if this runs under linux/macos
                    ProcessHelper.CmdExecute(@"tsc -p . --sourceMap false", hosthookDir);
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
                    buildPath = Path.Combine(Directory.GetCurrentDirectory(),parser.Arguments[_paramOutputDirectory][0]);
                }

                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                // ToDo: Need a solution for --asar support

                string electronArch = "x64";
                if (parser.Arguments.ContainsKey(_paramElectronArch))
                {
                    electronArch = parser.Arguments[_paramElectronArch][0];
                }

                string electronParams = "";
                if (parser.Arguments.ContainsKey(_paramElectronParams))
                {
                    electronParams = parser.Arguments[_paramElectronParams][0];
                }

                // ToDo: Make the same thing easer with native c# - we can save a tmp file in production code :)
                Console.WriteLine("Create electron-builder configuration file...");
                ProcessHelper.CmdExecute($"node build-helper.js", tempPath);

                Console.WriteLine("Setting up build script for electron-builder...");
                var electronBuilderScript = $"electron-builder . --config=./bin/electron-builder.json --platform={platformInfo.ElectronPackerPlatform} --arch={electronArch} {electronParams}";
                var packageJson = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(packageJsonPath));
                packageJson["scripts"]["electron-build"] = electronBuilderScript;
                File.WriteAllText(packageJsonPath, packageJson.ToString());

                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute("npm run electron-build", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }
    }
}
