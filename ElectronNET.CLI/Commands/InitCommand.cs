using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private static string ConfigName = "electron.manifest.json";
        private const string DefaultConfigFileName = "electron.manifest.json";

        public InitCommand(string[] args)
        {
            _parser.Parse(args);
        }

        private const string _aspCoreProjectPath = "project-path";
        private const string _manifest = "manifest";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                string aspCoreProjectPath = "";

                if (_parser.Arguments.ContainsKey(_aspCoreProjectPath))
                {
                    string projectPath = _parser.Arguments[_aspCoreProjectPath].First();
                    if (Directory.Exists(projectPath))
                    {
                        aspCoreProjectPath = projectPath;
                    }
                }
                else
                {
                    aspCoreProjectPath = Directory.GetCurrentDirectory();
                }

                var currentDirectory = aspCoreProjectPath;

                if(_parser.Arguments.ContainsKey(_manifest))
                {
                    ConfigName = "electron.manifest." + _parser.Arguments[_manifest].First() + ".json";
                    Console.WriteLine($"Adding your custom {ConfigName} config file to your project...");
                }
                else
                {
                    Console.WriteLine("Adding our config file to your project...");
                }

                var targetFilePath = Path.Combine(currentDirectory, ConfigName);

                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine("Config file already in your project.");
                    return false;
                }

                // Deploy config file
                EmbeddedFileHelper.DeployEmbeddedFileToTargetFile(currentDirectory, DefaultConfigFileName, ConfigName);

                // search .csproj/.fsproj (.csproj has higher precedence)
                Console.WriteLine($"Search your .csproj/fsproj to add the needed {ConfigName}...");
                var projectFile = Directory.EnumerateFiles(currentDirectory, "*.csproj", SearchOption.TopDirectoryOnly)
                    .Union(Directory.EnumerateFiles(currentDirectory, "*.fsproj", SearchOption.TopDirectoryOnly))
                    .FirstOrDefault();

                // update config file with the name of the csproj/fsproj
                // ToDo: If the csproj/fsproj name != application name, this will fail
                string text = File.ReadAllText(targetFilePath);
                text = text.Replace("{{executable}}", Path.GetFileNameWithoutExtension(projectFile));
                File.WriteAllText(targetFilePath, text);

                var extension = Path.GetExtension(projectFile);
                Console.WriteLine($"Found your {extension}: {projectFile} - check for existing config or update it.");

                if (!EditProjectFile(projectFile)) return false;

                // search launchSettings.json
                Console.WriteLine($"Search your .launchSettings to add our electron debug profile...");

                EditLaunchSettings(currentDirectory);

                Console.WriteLine($"Everything done - happy electronizing!");

                return true;
            });
        }

        private static void EditLaunchSettings(string currentDirectory)
        {
            // super stupid implementation, but because there is no nativ way to parse json
            // and cli extensions and other nuget packages are buggy 
            // this is should solve the problem for 80% of the users
            // for the other 20% we might fail... 
            var launchSettingFile = Path.Combine(currentDirectory, "Properties", "launchSettings.json");

            if (File.Exists(launchSettingFile) == false)
            {
                Console.WriteLine("launchSettings.json not found - do nothing.");
                return;
            }

            string launchSettingText = File.ReadAllText(launchSettingFile);

            if(_parser.Arguments.ContainsKey(_manifest))
            {
                string manifestName = _parser.Arguments[_manifest].First();

                if(launchSettingText.Contains("start /manifest " + ConfigName) == false)
                {
                    StringBuilder debugProfileBuilder = new StringBuilder();
                    debugProfileBuilder.AppendLine("profiles\": {");
                    debugProfileBuilder.AppendLine("    \"Electron.NET App - " + manifestName + "\": {");
                    debugProfileBuilder.AppendLine("      \"commandName\": \"Executable\",");
                    debugProfileBuilder.AppendLine("      \"executablePath\": \"electronize\",");
                    debugProfileBuilder.AppendLine("      \"commandLineArgs\": \"start /manifest " + ConfigName + "\",");
                    debugProfileBuilder.AppendLine("      \"workingDirectory\": \".\"");
                    debugProfileBuilder.AppendLine("    },");

                    launchSettingText = launchSettingText.Replace("profiles\": {", debugProfileBuilder.ToString());
                    File.WriteAllText(launchSettingFile, launchSettingText);

                    Console.WriteLine($"Debug profile added!");
                }
                else
                {
                    Console.WriteLine($"Debug profile already existing");
                }
            } 
            else if (launchSettingText.Contains("\"executablePath\": \"electronize\"") == false)
            {
                StringBuilder debugProfileBuilder = new StringBuilder();
                debugProfileBuilder.AppendLine("profiles\": {");
                debugProfileBuilder.AppendLine("    \"Electron.NET App\": {");
                debugProfileBuilder.AppendLine("      \"commandName\": \"Executable\",");
                debugProfileBuilder.AppendLine("      \"executablePath\": \"electronize\",");
                debugProfileBuilder.AppendLine("      \"commandLineArgs\": \"start\",");
                debugProfileBuilder.AppendLine("      \"workingDirectory\": \".\"");
                debugProfileBuilder.AppendLine("    },");

                launchSettingText = launchSettingText.Replace("profiles\": {", debugProfileBuilder.ToString());
                File.WriteAllText(launchSettingFile, launchSettingText);

                Console.WriteLine($"Debug profile added!");
            }
            else
            {
                Console.WriteLine($"Debug profile already existing");
            }
        }

        private static bool EditProjectFile(string projectFile)
        {
            using (var stream = File.Open(projectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var xmlDocument = XDocument.Load(stream);

                var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();
                if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web")
                {
                    Console.WriteLine(
                        $"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
                    return false;
                }

                if (xmlDocument.ToString().Contains($"Content Update=\"{ConfigName}\""))
                {
                    Console.WriteLine($"{ConfigName} already in csproj/fsproj.");
                    return false;
                }

                Console.WriteLine($"{ConfigName} will be added to csproj/fsproj.");

                string itemGroupXmlString = "<ItemGroup>" +
                                            "<Content Update=\"" + ConfigName + "\">" +
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
                using (XmlWriter xw = XmlWriter.Create(stream, xws))
                {
                    xmlDocument.Save(xw);
                }

            }

            Console.WriteLine($"{ConfigName} added in csproj/fsproj!");
            return true;
        }
    }
}
