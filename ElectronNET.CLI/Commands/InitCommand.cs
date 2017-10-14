using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ElectronNET.CLI.Commands
{
    public class InitCommand : ICommand
    {
        public const string COMMAND_NAME = "init";
        public const string COMMAND_DESCRIPTION = "Creates the needed Electron.NET config for your Electron Application.";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private const string ConfigName = "electronnet.json";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                var currentDirectory = Directory.GetCurrentDirectory();

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

                using (var stream = File.Open(projectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var xmlDocument = XDocument.Load(stream);

                    var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();
                    if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web")
                    {
                        Console.WriteLine($"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
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

                    xmlDocument.Save(stream);

                    Console.WriteLine($"{ConfigName} added in csproj - happy electronizing!");
                }

                return true;
            });
        }


    }
}
