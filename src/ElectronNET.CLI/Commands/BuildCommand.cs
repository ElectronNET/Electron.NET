using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
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
                                                 "Optional: '/no-restore' to disable nuget packages restore" + Environment.NewLine +
                                                 "Optional: '/electron-arch' to specify the resulting electron processor architecture (e.g. ia86 for x86 builds). Be aware to use the '/target custom' param as well!" + Environment.NewLine +
                                                 "Optional: '/electron-params' specify any other valid parameter, which will be routed to the electron-packager." + Environment.NewLine +
                                                 "Optional: '/relative-path' to specify output a subdirectory for output." + Environment.NewLine +
                                                 "Optional: '/absolute-path to specify and absolute path for output." + Environment.NewLine +
                                                 "Optional: '/package-json' to specify a custom package.json file." + Environment.NewLine +
                                                 "Optional: '/install-modules' to force node module install. Implied by '/package-json'" + Environment.NewLine +
                                                 "Optional: '/Version' to specify the version that should be applied to both the `dotnet publish` and `electron-builder` commands. Implied by '/Version'" + Environment.NewLine +
                                                 "Optional: '/p:[property]' or '/property:[property]' to pass in dotnet publish properties.  Example: '/property:Version=1.0.0' to override the FileVersion" + Environment.NewLine +
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
        private string _paramSelfContained = "SelfContained";
        private string _paramNoRestore = "no-restore";
        private string _paramVersion = "Version";

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

                string noRestore = parser.Arguments.ContainsKey(_paramNoRestore)
                    ? " --no-restore"
                    : string.Empty;

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

                var dotNetPublishFlags = GetDotNetPublishFlags(parser);

                var command =
                    $"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\"{noRestore} --output \"{tempBinPath}\" {string.Join(' ', dotNetPublishFlags.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";

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

                // Read electron version from manifest file and update package.json BEFORE npm install
                string manifestFileName = "electron.manifest.json";

                if (parser.Arguments.ContainsKey(_manifest))
                {
                    manifestFileName = parser.Arguments[_manifest].First();
                }

                // Read electron version from manifest file
                string electronVersion = "23.2.0"; // default fallback version
                string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), manifestFileName);
                
                Console.WriteLine($"Reading electronVersion from manifest file: {manifestPath}");
                
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
                                else
                                {
                                    Console.WriteLine($"electronVersion property found but empty in manifest file, using fallback version {electronVersion}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"electronVersion property not found in manifest file, using fallback version {electronVersion}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not read electronVersion from manifest file: {ex.Message}");
                        Console.WriteLine($"Using fallback Electron version {electronVersion}");
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Manifest file not found at {manifestPath}");
                    Console.WriteLine($"Using fallback Electron version {electronVersion}");
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

                var checkForNodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                if (Directory.Exists(checkForNodeModulesDirPath) == false || parser.Contains(_paramForceNodeInstall) || parser.Contains(_paramPackageJson))
                    Console.WriteLine("Start npm install...");

                ProcessHelper.CmdExecute("npm install --production", tempPath);

                // Update build helper and configuration files after npm install
                Console.WriteLine("Create electron-builder configuration file...");
                ProcessHelper.CmdExecute(
                    string.IsNullOrWhiteSpace(version)
                        ? $"node build-helper.js {manifestFileName}"
                        : $"node build-helper.js {manifestFileName} {version}", tempPath);

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

                Console.WriteLine($"Package Electron App for Platform {platformInfo.ElectronPackerPlatform}...");
                ProcessHelper.CmdExecute($"npx electron-builder --config=./bin/electron-builder.json --{platformInfo.ElectronPackerPlatform} --{electronArch} -c.electronVersion={electronVersion} {electronParams}", tempPath);

                Console.WriteLine("... done");

                return true;
            });
        }

        private Dictionary<string, string> GetDotNetPublishFlags(SimpleCommandLineParser parser)
        {
            var dotNetPublishFlags = new Dictionary<string, string>
            {
                {"/p:PublishReadyToRun", parser.TryGet(_paramPublishReadyToRun, out var rtr) ? rtr[0] : "true"},
                {"/p:PublishSingleFile", parser.TryGet(_paramPublishSingleFile, out var psf) ? psf[0] : "true"},
                {"/p:SelfContained", parser.TryGet(_paramSelfContained, out var sc) ? sc[0] : "true"},
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
