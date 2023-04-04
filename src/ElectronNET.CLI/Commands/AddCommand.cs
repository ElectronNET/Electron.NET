using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ElectronNET.CLI.Commands
{
    public class AddCommand : ICommand
    {
        public const string COMMAND_NAME = "add";
        public const string COMMAND_DESCRIPTION = "The add command needs to be invoked via 'add hosthook'. This creates a special folder for your custom npm package installation.";
        public const string COMMAND_ARGUMENTS = "hosthook";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public AddCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if (_args.Length == 0)
                {
                    Console.WriteLine("Specify 'hosthook' to add custom npm packages.");
                    return false;
                }

                if (_args[0].ToLowerInvariant() != "hosthook")
                {
                    Console.WriteLine("Specify 'hosthook' to add custom npm packages.");
                    return false;
                }

                // Maybe ToDo: Adding the possiblity to specify a path (like we did in the InitCommand, but this would require a better command args parser)
                var currentDirectory = Directory.GetCurrentDirectory();
                var hostFolder = Path.Combine(currentDirectory, "ElectronHostHook");
                
                if (!Directory.Exists(hostFolder))
                {
                    Directory.CreateDirectory(hostFolder);
                }

                // Deploy related files
                EmbeddedFileHelper.DeployEmbeddedFile(hostFolder, "package.json", "hook.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostFolder, "tsconfig.json", "hook.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostFolder, ".gitignore", "hook.");
                EmbeddedFileHelper.DeployEmbeddedFile(hostFolder, "index.ts", "hook.");

                Console.WriteLine($"Installing the dependencies ...");
                ProcessHelper.CheckNodeModules(hostFolder);

                // search .csproj or .fsproj (.csproj has higher precedence)
                Console.WriteLine($"Search your .csproj/.fsproj to add configure CopyToPublishDirectory to 'Never'");

                var projectFile = Directory
                    .EnumerateFiles(currentDirectory, "*.csproj", SearchOption.TopDirectoryOnly)
                    .Union(Directory.EnumerateFiles(currentDirectory, "*.fsproj", SearchOption.TopDirectoryOnly))
                    .FirstOrDefault();

                var extension = Path.GetExtension(projectFile);
                Console.WriteLine($"Found your {extension}: {projectFile} - check for existing CopyToPublishDirectory setting or update it.");

                if (!EditProjectFile(projectFile)) 
                {
                    return false;
                }

                Console.WriteLine($"Everything done - happy electronizing with your custom npm packages!");
                return true;
            });
        }

        // ToDo: Cleanup this copy/past code.
        private static bool EditProjectFile(string projectFile)
        {
            using (var stream = File.Open(projectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var xmlDocument = XDocument.Load(stream);
                var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();

                if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web")
                {
                    Console.WriteLine($"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
                    return false;
                }

                var itemGroupXmlString = "<ItemGroup>" +
                                            "<Content Update=\"ElectronHostHook\\**\\*.*\">" +
                                               "<CopyToPublishDirectory>Never</CopyToPublishDirectory>" +
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

            Console.WriteLine($"Publish setting added in csproj/fsproj!");
            return true;
        }

    }
}
