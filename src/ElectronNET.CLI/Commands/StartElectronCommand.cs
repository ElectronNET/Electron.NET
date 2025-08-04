using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.Json;
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
                {
                    Directory.CreateDirectory(tempPath);
                }

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

                DeployEmbeddedElectronFiles.Do(tempPath);

                // Update package.json with electronVersion from manifest before npm install
                string manifestFileName = "electron.manifest.json";
                if (parser.Arguments.ContainsKey(_manifest))
                {
                    manifestFileName = parser.Arguments[_manifest].First();
                }

                // Read electron version from manifest file
                string electronVersion = "23.2.0"; // default fallback version
                string manifestPath = Path.Combine(aspCoreProjectPath, manifestFileName);
                
                if (File.Exists(manifestPath))
                {
                    try
                    {
                        string manifestContent = File.ReadAllText(manifestPath);
                        using (JsonDocument document = JsonDocument.Parse(manifestContent))
                        {
                            if (document.RootElement.TryGetProperty("electronVersion", out JsonElement electronVersionElement))
                            {
                                string manifestElectronVersion = electronVersionElement.GetString();
                                if (!string.IsNullOrWhiteSpace(manifestElectronVersion))
                                {
                                    electronVersion = manifestElectronVersion;
                                    Console.WriteLine($"Using Electron version {electronVersion} from manifest file");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not read electronVersion from manifest file: {ex.Message}");
                    }
                }

                // Update package.json with electronVersion directly in C# before npm install
                string packageJsonPath = Path.Combine(tempPath, "package.json");
                if (File.Exists(packageJsonPath))
                {
                    try
                    {
                        string packageJsonContent = File.ReadAllText(packageJsonPath);
                        using (JsonDocument packageDocument = JsonDocument.Parse(packageJsonContent))
                        {
                            var packageJsonObject = new Dictionary<string, object>();
                            
                            // Copy existing properties
                            foreach (var property in packageDocument.RootElement.EnumerateObject())
                            {
                                if (property.Name == "devDependencies")
                                {
                                    var devDeps = new Dictionary<string, object>();
                                    foreach (var dep in property.Value.EnumerateObject())
                                    {
                                        devDeps[dep.Name] = dep.Value.GetString();
                                    }
                                    // Update electron version
                                    devDeps["electron"] = $"^{electronVersion}";
                                    packageJsonObject["devDependencies"] = devDeps;
                                }
                                else if (property.Value.ValueKind == JsonValueKind.String)
                                {
                                    packageJsonObject[property.Name] = property.Value.GetString();
                                }
                                else if (property.Value.ValueKind == JsonValueKind.Object)
                                {
                                    var subObject = new Dictionary<string, object>();
                                    foreach (var subProp in property.Value.EnumerateObject())
                                    {
                                        subObject[subProp.Name] = subProp.Value.GetString();
                                    }
                                    packageJsonObject[property.Name] = subObject;
                                }
                                else
                                {
                                    packageJsonObject[property.Name] = property.Value.ToString();
                                }
                            }
                            
                            // Ensure devDependencies exists and contains electron
                            if (!packageJsonObject.ContainsKey("devDependencies"))
                            {
                                packageJsonObject["devDependencies"] = new Dictionary<string, object>();
                            }
                            var devDependencies = (Dictionary<string, object>)packageJsonObject["devDependencies"];
                            devDependencies["electron"] = $"^{electronVersion}";
                            
                            string updatedPackageJson = JsonSerializer.Serialize(packageJsonObject, new JsonSerializerOptions 
                            { 
                                WriteIndented = true 
                            });
                            
                            File.WriteAllText(packageJsonPath, updatedPackageJson);
                            Console.WriteLine($"Updated package.json with Electron version {electronVersion}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not update package.json with electronVersion: {ex.Message}");
                    }
                }

                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                Console.WriteLine("node_modules missing in: " + nodeModulesDirPath);

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                // Execute build-helper.js to setup other configurations after npm install
                ProcessHelper.CmdExecute($"node build-helper.js {manifestFileName}", tempPath);

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
