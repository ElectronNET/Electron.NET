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

        private static string ElectronHostHookFolderName = "ElectronHostHook";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                if(_args.Length == 0)
                {
                    Console.WriteLine("Specify 'hosthook' to add custom npm packages.");
                    return false;
                }

                if(_args[0].ToLowerInvariant() != "hosthook")
                {
                    Console.WriteLine("Specify 'hosthook' to add custom npm packages.");
                    return false;
                }

                string aspCoreProjectPath = "";

                // Maybe ToDo: Adding the possiblity to specify a path (like we did in the InitCommand, but this would require a better command args parser)
                aspCoreProjectPath = Directory.GetCurrentDirectory();

                var currentDirectory = aspCoreProjectPath;

                var targetFilePath = Path.Combine(currentDirectory, ElectronHostHookFolderName);

                if(Directory.Exists(targetFilePath))
                {
                    Console.WriteLine("ElectronHostHook directory already in place. If you want to start over, delete the folder and invoke this command again.");
                    return false;
                }

                Console.WriteLine("Adding the ElectronHostHook folder to your project...");

                Directory.CreateDirectory(targetFilePath);

                // Deploy related files
                EmbeddedFileHelper.DeployEmbeddedFile(targetFilePath, "index.ts", "ElectronHostHook.");
                EmbeddedFileHelper.DeployEmbeddedFile(targetFilePath, "connector.ts", "ElectronHostHook.");
                EmbeddedFileHelper.DeployEmbeddedFile(targetFilePath, "package.json", "ElectronHostHook.");
                EmbeddedFileHelper.DeployEmbeddedFile(targetFilePath, "tsconfig.json", "ElectronHostHook.");
                EmbeddedFileHelper.DeployEmbeddedFile(targetFilePath, ".gitignore", "ElectronHostHook.");

                // npm for typescript compiler etc.
                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", targetFilePath);

                // run typescript compiler
                string tscPath = Path.Combine(targetFilePath, "node_modules", ".bin");
                // ToDo: Not sure if this runs under linux/macos
                ProcessHelper.CmdExecute(@"tsc -p ../../", tscPath);

                // search .csproj
                Console.WriteLine($"Search your .csproj to add configure CopyToPublishDirectory to 'Never'");
                var projectFile = Directory.EnumerateFiles(currentDirectory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

                Console.WriteLine($"Found your .csproj: {projectFile} - check for existing CopyToPublishDirectory setting or update it.");

                if (!EditCsProj(projectFile)) return false;

                Console.WriteLine($"Everything done - happy electronizing with your custom npm packages!");

                return true;
            });
        }

        // ToDo: Cleanup this copy/past code.
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

                string itemGroupXmlString = "<ItemGroup>" +
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

            Console.WriteLine($"Publish setting added in csproj!");
            return true;
        }

    }
}
