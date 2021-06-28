using System;
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
        public static string COMMAND_ARGUMENTS = "Needed: '/target' with params 'win/osx/linux' to build for a typical app or use 'custom' and specify .NET Core build config & electron build config" + Environment.NewLine +
                                                 " for custom target, check .NET Core RID Catalog and Electron build target/" + Environment.NewLine +
                                                 " e.g. '/target win' or '/target custom \"win7-x86;win32\"'" + Environment.NewLine +
                                                 "Optional: '/dotnet-configuration' with the desired .NET Core build config e.g. release or debug. Default = Release" + Environment.NewLine +
                                                 "Optional: '/electron-arch' to specify the resulting electron processor architecture (e.g. ia86 for x86 builds). Be aware to use the '/target custom' param as well!" + Environment.NewLine +
                                                 "Optional: '/electron-params' specify any other valid parameter, which will be routed to the electron-packager." + Environment.NewLine +
                                                 "Optional: '/relative-path' to specify output a subdirectory for output." + Environment.NewLine +
                                                 "Optional: '/absolute-path to specify and absolute path for output." + Environment.NewLine +
                                                 "Optional: '/package-json' to specify a custom package.json file." + Environment.NewLine +
                                                 "Optional: '/install-modules' to force node module install. Implied by '/package-json'" + Environment.NewLine +
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
        private string _manifest = "manifest";
        private string _paramPublishReadyToRun = "PublishReadyToRun";
        private string _paramPublishSingleFile = "PublishSingleFile";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Electron Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser();
                parser.Parse(_args);

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

                var resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {publishReadyToRun} {publishSingleFile} --self-contained", Directory.GetCurrentDirectory());

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

                if (Directory.Exists(checkForNodeModulesDirPath) == false || parser.Contains(_paramForceNodeInstall) || parser.Contains(_paramPackageJson))

                    Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install --production", tempPath);

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

                ProcessHelper.CmdExecute($"node build-helper.js " + manifestFileName, tempPath);

                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute($"npx electron-builder --config=./bin/electron-builder.json --{platformInfo.ElectronPackerPlatform} --{electronArch} -c.electronVersion=12.0.12 {electronParams}", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }
    }
}