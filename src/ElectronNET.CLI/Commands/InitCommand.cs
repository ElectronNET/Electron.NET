using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ElectronNET.CLI.Commands
{
    public class InitCommand : ICommand
    {
        public const string COMMAND_NAME = "init";
        public const string COMMAND_DESCRIPTION = "Creates the needed Electron.NET config for your Electron Application.";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private static SimpleCommandLineParser _parser = new SimpleCommandLineParser();
        private const string DefaultConfigFileName = "electron.manifest.json";

        public InitCommand(string[] args) => _parser.Parse(args);

        private static string _aspCoreProjectPath = "project-path";
        private static string _manifest = "manifest";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                var currentDirectory = GetProjectPath(_aspCoreProjectPath);

                if (string.IsNullOrEmpty(currentDirectory)) return false;

                var configName = "electron.manifest.json";
                if (_parser.Arguments.TryGetValue(_manifest, out var manifestData))
                {
                    configName = $"electron.manifest.{manifestData.First()}.json";
                    Console.WriteLine($"Adding your custom {configName} config file to your project...");
                }
                else
                {
                    Console.WriteLine("Adding our config file to your project...");
                }

                var targetFilePath = Path.Combine(currentDirectory, configName);

                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine("Config file already in your project.");
                    return false;
                }

                // Deploy config file
                EmbeddedFileHelper.DeployEmbeddedFileToTargetFile(currentDirectory, DefaultConfigFileName, configName);

                // search .csproj/.fsproj (.csproj has higher precedence)
                Console.WriteLine($"Search your .csproj/fsproj to add the needed {configName}...");
                var projectFile = Directory.EnumerateFiles(currentDirectory, "*.csproj", SearchOption.TopDirectoryOnly)
                    .Union(Directory.EnumerateFiles(currentDirectory, "*.fsproj", SearchOption.TopDirectoryOnly))
                    .FirstOrDefault();

                // update config file with the name of the csproj/fsproj
                // ToDo: If the csproj/fsproj name != application name, this will fail
                var text = File.ReadAllText(targetFilePath);
                text = text.Replace("{{executable}}", Path.GetFileNameWithoutExtension(projectFile));
                File.WriteAllText(targetFilePath, text);

                Console.WriteLine($"Found your {Path.GetExtension(projectFile)}: {projectFile} - check for existing config or update it.");

                if (!EditProjectFile(projectFile, configName)) return false;

                // search launchSettings.json
                Console.WriteLine($"Search your .launchSettings to add our electron debug profile...");

                if (!EditLaunchSettings(currentDirectory, configName)) return false;

                Console.WriteLine($"Everything done - happy electronizing!");

                return true;
            });
        }

        private static string GetProjectPath(string projectPathData)
        {
            var aspCoreProjectPath = Directory.GetCurrentDirectory();

            if (_parser.Arguments.TryGetValue(projectPathData, out var argument))
            {
                var directoryInfo = new DirectoryInfo(argument.First());
                if (!directoryInfo.Exists)
                {
                    // Create project if project not exist
                    if (directoryInfo.Parent != null)
                    {
                        Directory.CreateDirectory(directoryInfo.Parent.FullName);
                        ProcessHelper.CmdExecute($"dotnet new webapp -o {directoryInfo.Name}", directoryInfo.Parent.FullName);

                        return directoryInfo.FullName;
                    }

                    Console.WriteLine("Unable to resolve directory");

                    return string.Empty;
                }
            }
            
            return CheckASPProject(aspCoreProjectPath) ? aspCoreProjectPath : string.Empty;
        }

        private static bool CheckASPProject(string projectFile)
        {
            try
            {
                using var stream = File.Open(projectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                var xmlDocument = XDocument.Load(stream);

                var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();
                if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web")
                {
                    Console.WriteLine($"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static bool EditLaunchSettings(string currentDirectory, string configName)
        {
            var launchSettingFile = Path.Combine(currentDirectory, "Properties", "launchSettings.json");

            if (File.Exists(launchSettingFile) == false)
            {
                Console.WriteLine("launchSettings.json not found - do nothing.");
                return false;
            }

            var launchSettingText = File.ReadAllText(launchSettingFile);
            var launchSettingJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(launchSettingText);

            var profileName = _parser.Arguments.TryGetValue(_manifest, out var manifestData)
                ? $"Electron.NET App - {manifestData.First()}"
                : "Electron.NET App";

            var isProfilesExist = launchSettingJson.TryGetValue("profiles", out var profilesData);
            var profileJson = profilesData.Deserialize<Dictionary<string, JsonElement>>();
            var isProfileExist = profileJson.TryGetValue(profileName, out var profileData);

            if (isProfilesExist && isProfileExist)
            {
                Console.WriteLine($"Debug profile already existing");
                return false;
            }

            var profileConfigData = profileData.Deserialize<Dictionary<string, string>>();
            profileConfigData["commandName"] = "Executable";
            profileConfigData["commandName"] = "Executable";
            profileConfigData["executablePath"] = "electronize";
            profileConfigData["commandLineArgs"] = _parser.Arguments.TryGetValue(_manifest, out _) ? $"start /manifest {configName}" : "start";
            profileConfigData["workingDirectory"] = ".";
            profileJson[profileName] = JsonSerializer.SerializeToElement(profileConfigData);
            launchSettingJson["profiles"] = JsonSerializer.SerializeToElement(profileJson);

            File.WriteAllText(launchSettingFile, JsonSerializer.Serialize(launchSettingJson, new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine($"Debug profile added!");

            return true;
        }

        private static bool EditProjectFile(string projectFile, string configName)
        {
            using (var stream = File.Open(projectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var xmlDocument = XDocument.Load(stream);
                
                if (xmlDocument.ToString().Contains($"Content Update=\"{configName}\""))
                {
                    Console.WriteLine($"{configName} already in csproj/fsproj.");
                    return false;
                }

                Console.WriteLine($"{configName} will be added to csproj/fsproj.");

                string itemGroupXmlString = "<ItemGroup>" +
                                            "<Content Update=\"" + configName + "\">" +
                                            "<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" +
                                            "</Content>" +
                                            "</ItemGroup>";

                var newItemGroupForConfig = XElement.Parse(itemGroupXmlString);
                xmlDocument.Root.Add(newItemGroupForConfig);

                stream.SetLength(0);
                stream.Position = 0;

                var xws = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true
                };
                using (var xw = XmlWriter.Create(stream, xws))
                {
                    xmlDocument.Save(xw);
                }
            }

            Console.WriteLine($"{configName} added in csproj/fsproj!");

            return true;
        }
    }
}
