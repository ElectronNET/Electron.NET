using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private const string ConfigName = "electron.manifest.json";

        private string[] _args;

        public InitCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                string aspCoreProjectPath = "";

                if (_args.Length > 0)
                {
                    if (Directory.Exists(_args[0]))
                    {
                        aspCoreProjectPath = _args[0];
                    }
                }
                else
                {
                    aspCoreProjectPath = Directory.GetCurrentDirectory();
                }

                var currentDirectory = aspCoreProjectPath;

                Console.WriteLine("Adding our config file to your project...");

                var targetFilePath = Path.Combine(currentDirectory, ConfigName);

                if (File.Exists(targetFilePath))
                {
                    Console.WriteLine("Config file already in your project.");
                    return false;
                }

                // Deploy config file
                EmbeddedFileHelper.DeployEmbeddedFile(currentDirectory, ConfigName);

                // search .csproj
                Console.WriteLine($"Search your .csproj to add the needed {ConfigName}...");
                var projectFile = Directory.EnumerateFiles(currentDirectory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

                // update config file with the name of the csproj
                // ToDo: If the csproj name != application name, this will fail
                string text = File.ReadAllText(targetFilePath);
                text = text.Replace("{{executable}}", Path.GetFileNameWithoutExtension(projectFile));
                File.WriteAllText(targetFilePath, text);

                Console.WriteLine($"Found your .csproj: {projectFile} - check for existing config or update it.");

                if (!EditCsProj(projectFile)) return false;

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

            if (launchSettingText.Contains("electronize start") == false)
            {
                StringBuilder debugProfileBuilder = new StringBuilder();
                debugProfileBuilder.AppendLine("profiles\": {");
                debugProfileBuilder.AppendLine("\"Electron.NET App\": {");
                debugProfileBuilder.AppendLine("\"commandName\": \"Executable\",");
                debugProfileBuilder.AppendLine("\"executablePath\": \"C:\\\\Program Files\\\\dotnet\\\\dotnet.exe\",");
                debugProfileBuilder.AppendLine("\"commandLineArgs\": \"electronize start\"");
                debugProfileBuilder.AppendLine("},");

                launchSettingText = launchSettingText.Replace("profiles\": {", debugProfileBuilder.ToString());
                File.WriteAllText(launchSettingFile, launchSettingText);

                Console.WriteLine($"Debug profile added!");
            }
            else
            {
                Console.WriteLine($"Debug profile already existing");
            }
        }

        private static bool EditCsProj(string projectFile)
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
                    Console.WriteLine($"{ConfigName} already in csproj.");
                    return false;
                }

                Console.WriteLine($"{ConfigName} will be added to csproj.");

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

            Console.WriteLine($"{ConfigName} added in csproj!");
            return true;
        }
    }
}
