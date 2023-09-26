using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                                                 " e.g. '/target win' or '/target custom \"win7-x86;win\"'" + Environment.NewLine +
                                                 "Optional: '/dotnet-configuration' with the desired .NET Core build config e.g. release or debug. Default = Release" + Environment.NewLine +
                                                 "Optional: '/electron-arch' to specify the resulting electron processor architecture (e.g. ia86 for x86 builds). Be aware to use the '/target custom' param as well!" + Environment.NewLine +
                                                 "Optional: '/electron-params' specify any other valid parameter, which will be routed to the electron-packager." + Environment.NewLine +
                                                 "Optional: '/relative-path' to specify output a subdirectory for output." + Environment.NewLine +
                                                 "Optional: '/absolute-path to specify and absolute path for output." + Environment.NewLine +
                                                 "Optional: '/package-json' to specify a custom package.json file." + Environment.NewLine +
                                                 "Optional: '/Version' to specify the version that should be applied to both the `dotnet publish` and `electron-builder` commands. Implied by '/Version'" + Environment.NewLine +
                                                 "Optional: '/p:[property]' or '/property:[property]' to pass in dotnet publish properties.  Example: '/property:Version=1.0.0' to override the FileVersion" + Environment.NewLine +
                                                 "Full example for a 32bit debug build with electron prune: build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params \"--prune=true \"";

        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private static SimpleCommandLineParser _parser = new();

        public BuildCommand(string[] args) => _parser.Parse(args);

        private string _paramTarget = "target";
        private string _paramDotNetConfig = "dotnet-configuration";
        private string _paramElectronArch = "electron-arch";
        private string _paramElectronParams = "electron-params";
        private string _paramOutputDirectory = "relative-path";
        private string _paramAbsoluteOutput = "absolute-path";
        private string _paramPackageJson = "package-json";
        private string _manifest = "manifest";
        private string _paramPublishReadyToRun = "PublishReadyToRun";
        private string _paramPublishSingleFile = "PublishSingleFile";
        private string _paramVersion = "Version";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Build Electron Application...");

                //This version will be shared between the dotnet publish and electron-builder commands
                string version = _parser.Arguments.TryGetValue(_paramVersion, out string[] versionData) ? versionData[0] : string.Empty;

                if (!_parser.Arguments.ContainsKey(_paramTarget))
                {
                    Console.WriteLine($"Error: missing '{_paramTarget}' argument.");
                    Console.WriteLine(COMMAND_ARGUMENTS);
                    return false;
                }

                var desiredPlatform = _parser.Arguments[_paramTarget][0];
                string specifiedFromCustom = string.Empty;
                if (desiredPlatform == "custom" && _parser.Arguments[_paramTarget].Length > 1)
                {
                    specifiedFromCustom = _parser.Arguments[_paramTarget][1];
                }

                string configuration = _parser.Arguments.TryGetValue(_paramDotNetConfig, out string[] configData) ? configData[0] : "Release";

                var platformInfo = GetTargetPlatformInformation.Do(desiredPlatform, specifiedFromCustom);

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid}...");

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "obj", "desktop", desiredPlatform);

                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }
                else
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Attempt to reset directory permissions and try again.
                        var di = new DirectoryInfo(tempPath);
                        di.Attributes &= ~FileAttributes.ReadOnly;
                        foreach (var dir in di.GetDirectories())
                        {
                            dir.Attributes &= ~FileAttributes.ReadOnly;
                        }
                        foreach (var file in di.GetFiles())
                        {
                            file.Attributes &= ~FileAttributes.ReadOnly;
                        }
                        Directory.Delete(tempPath, true);
                    }
                    Directory.CreateDirectory(tempPath);
                }

                Console.WriteLine("Executing dotnet publish in this directory: " + tempPath);

                string tempBinPath = Path.Combine(tempPath, "bin");

                Console.WriteLine($"Build ASP.NET Core App for {platformInfo.NetCorePublishRid} under {configuration}-Configuration...");

                Dictionary<string, string> dotNetPublishFlags = GetDotNetPublishFlags(_parser);

                string command =
                    $"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {string.Join(' ', dotNetPublishFlags.Select(kvp => $"{kvp.Key}={kvp.Value}"))} --self-contained";

                // output the command 
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(command);
                Console.ResetColor();

                int resultCode = ProcessHelper.CmdExecute(command, Directory.GetCurrentDirectory());

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                string[] hostHookFolders = Directory.GetDirectories(Directory.GetCurrentDirectory(), "ElectronHostHook", SearchOption.AllDirectories);

                DeployEmbeddedElectronFiles.Do(tempPath, hostHookFolders.Length > 0);

                if (_parser.Arguments.TryGetValue(_paramPackageJson, out string[] packageData))
                {
                    Console.WriteLine("Copying custom package.json.");

                    File.Copy(packageData[0], Path.Combine(tempPath, "package.json"), true);
                }

                Console.WriteLine("ElectronHostHook handling started...");

                if (hostHookFolders.Length > 0)
                {
                    string hostHookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(hostHookFolders.First(), hostHookDir, true, new List<string>() { "node_modules" });

                    string package = Path.Combine(hostHookDir, "package.json");

                    var jsonText = File.ReadAllText(package);

                    JsonDocument jsonDoc = JsonDocument.Parse(jsonText);
                    JsonElement root = jsonDoc.RootElement;

                    var packageJson = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonText);

                    packageJson["name"] = "@electron-host/hook";

                    string output = JsonSerializer.Serialize(packageJson, new JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText(package, output);
                }

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("Build Electron Desktop Application...");

                // Specifying an absolute path supercedes a relative path
                string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "desktop");
                if (_parser.Arguments.TryGetValue(_paramAbsoluteOutput, out string[] outputPathData))
                    buildPath = outputPathData[0];
                else if (_parser.Arguments.TryGetValue(_paramOutputDirectory, out string[] outputDirectoryData))
                    buildPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectoryData[0]);

                Console.WriteLine("Executing electron magic in this directory: " + buildPath);

                string electronArch = _parser.Arguments.TryGetValue(_paramElectronArch, out string[] archData) ? archData[0] : "x64";

                string electronParams = _parser.Arguments.TryGetValue(_paramElectronParams, out string[] paramData) ? paramData[0] : string.Empty;

                // ToDo: Make the same thing easer with native c# - we can save a tmp file in production code :)
                Console.WriteLine("Create electron-builder configuration file...");

                string manifestFileName = _parser.Arguments.TryGetValue(_manifest, out string[] manifestData) ? manifestData.First() : "electron.manifest.json";

                if (!ProcessManifest(tempPath, manifestFileName, version))
                    throw new Exception("Fail to update manifest.");

                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute($"npx electron-builder --config=./bin/electron-builder.json --{platformInfo.ElectronPackerPlatform} --{electronArch} -c.electronVersion=26.2.0 {electronParams}", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }

        private Dictionary<string, string> GetDotNetPublishFlags(SimpleCommandLineParser parser)
        {
            var dotNetPublishFlags = new Dictionary<string, string>
            {
                {"/p:PublishReadyToRun", parser.TryGet(_paramPublishReadyToRun, out string[] rtr) ? rtr[0] : "true"},
                {"/p:PublishSingleFile", parser.TryGet(_paramPublishSingleFile, out string[] psf) ? psf[0] : "true"},
            };

            if (parser.Arguments.ContainsKey(_paramVersion))
            {
                if (parser.Arguments.Keys.All(key => !key.StartsWith("p:Version=") && !key.StartsWith("property:Version=")))
                    dotNetPublishFlags.Add("/p:Version", parser.Arguments[_paramVersion][0]);
                if (parser.Arguments.Keys.All(key => !key.StartsWith("p:ProductVersion=") && !key.StartsWith("property:ProductVersion=")))
                    dotNetPublishFlags.Add("/p:ProductVersion", parser.Arguments[_paramVersion][0]);
            }

            foreach (var param in parser.Arguments.Keys.Where(key => key.StartsWith("p:") || key.StartsWith("property:")))
            {
                var split = param.IndexOf('=');
                if (split < 0) continue;

                var key = $"/{param.Substring(0, split)}";
                // normalize the key
                if (key.StartsWith("/property:"))
                    key = key.Replace("/property:", "/p:");

                var value = param[(split + 1)..];

                dotNetPublishFlags[key] = value;
            }

            return dotNetPublishFlags;
        }

        private bool ProcessManifest(string basePath, string manifestFileName, string buildVersion)
        {
            var manifestFilePath = Path.Combine(basePath, "bin", manifestFileName);
            var manifestFile = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(manifestFilePath));

            if (manifestFile.TryGetValue("build", out var rawBuilderConfiguration))
            {
                var builderConfiguration = JsonSerializer.Deserialize<Dictionary<string, object>>((JsonElement)rawBuilderConfiguration);
                if (!string.IsNullOrWhiteSpace(buildVersion))
                    builderConfiguration["buildVersion"] = buildVersion;

                UpdateJsonFile(Path.Combine(basePath, "package.json"), manifestFile, builderConfiguration);
                if (File.Exists(Path.Combine(basePath, "package-lock.json")))
                {
                    UpdateJsonFile(Path.Combine(basePath, "package-lock.json"), manifestFile, builderConfiguration);
                }

                var builderConfigurationString = JsonSerializer.Serialize(builderConfiguration, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path.Combine(basePath, "bin", "electron-builder.json"), builderConfigurationString);

                var manifestContent = JsonSerializer.Serialize(manifestFile, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path.Combine(basePath, "bin", "electron.manifest.json"), manifestContent);

                return true;
            }

            return false;
        }

        private static void UpdateJsonFile(string filePath, Dictionary<string, object> manifestFile, Dictionary<string, object> builderConfiguration)
        {
            var packageJson = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(filePath));

            packageJson["name"] = manifestFile.GetValueOrDefault("name", "electron-net").ToString().ToDashCase();
            packageJson["author"] = manifestFile.GetValueOrDefault("author", string.Empty);
            packageJson["version"] = builderConfiguration["buildVersion"]; // Must be exist
            packageJson["description"] = manifestFile.GetValueOrDefault("description", "");

            File.WriteAllText(filePath, JsonSerializer.Serialize(packageJson, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public static class StringExtensions
    {
        public static string ToDashCase(this string input)
        {
            return Regex.Replace(input, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}