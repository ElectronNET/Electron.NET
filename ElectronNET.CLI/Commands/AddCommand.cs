using ElectronNET.CLI.Config;
using ElectronNET.CLI.Config.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ElectronNET.CLI.Commands {

    /// <summary> Add Command. </summary>
    public class AddCommand : ICommand {

        /// <summary> General Application Settings. </summary>
        /// <value> General Application Settings. </value>
        private AppSettings appcfg { get; set; }

        /// <summary> Command specific settings. </summary>
        /// <value> Command specific settings. </value>
        private AddConfig cmdcfg { get; set; }


        /// <summary> Add Command Execute. </summary>
        /// <returns> Add Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {

                // Read in the configuration
                appcfg = SettingsLoader.Settings;
                cmdcfg = (AddConfig) appcfg.CommandConfig;

                switch (cmdcfg.SubCommand) {
                    case "hosthook":
                        if (!AddHosthook_DeployFiles())
                            return false;
                        if (!AddHosthook_EditCsProj())
                            return false;
                        break;
                    default:
                        return false;
                }
                Console.WriteLine($"Everything done - happy electronizing with your custom npm packages!");
                return true;
            });
        }


        /// <summary> Adds the ElectronHostHook files. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool AddHosthook_DeployFiles() {
            Console.WriteLine("Add hosthook files...");

            var targetFilePath = cmdcfg.ElectronHostHookPath;
            if (Directory.Exists(targetFilePath)) {
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
            Console.WriteLine($"Start {cmdcfg.NpmCommand.ToInstallCmd()}...");
            ProcessHelper.CmdExecute(cmdcfg.NpmCommand.ToInstallCmd(), targetFilePath);

            // run typescript compiler
            var tscPath = Path.Combine(targetFilePath, "node_modules", ".bin");
            // ToDo: Not sure if this runs under linux/macos
            ProcessHelper.CmdExecute(@"tsc -p ../../", tscPath);

            return true;
        }




        // ToDo: Cleanup this copy/past code.
        private bool AddHosthook_EditCsProj() {
            Console.WriteLine($"Update the .csproj to add configure CopyToPublishDirectory to 'Never'");

            // Find the .csproj file
            if (cmdcfg.ProjectFile == null) {
                Console.WriteLine("Searching for Project file");
                cmdcfg.ProjectFile = Directory
                    .EnumerateFiles(cmdcfg.ProjectPath, "*.csproj",
                        SearchOption.TopDirectoryOnly).FirstOrDefault();
            }
            if (cmdcfg.ProjectFile == null || File.Exists(cmdcfg.ProjectFile)) {
                Console.WriteLine("Error unable to locate .csproj file");
                return false;
            }
            Console.WriteLine($"Project file found: {cmdcfg.ProjectFile}");

            using (var stream = File.Open(cmdcfg.ProjectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                var xmlDocument = XDocument.Load(stream);

                var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();
                if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web") {
                    Console.WriteLine(
                        $"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
                    return false;
                }

                var itemGroupXmlString = "<ItemGroup>" +
                                                "<Content Update=\"ElectronHostHook\\**\\*.*\">" +
                                                    "<CopyToPublishDirectory>Never</CopyToPublishDirectory>" +
                                                "</Content>" +
                                            "</ItemGroup>";

                var newItemGroupForConfig = XElement.Parse(itemGroupXmlString);
                xmlDocument.Root?.Add(newItemGroupForConfig);

                stream.SetLength(0);
                stream.Position = 0;

                var xws = new XmlWriterSettings {
                    OmitXmlDeclaration = true,
                    Indent = true
                };
                using (var xw = XmlWriter.Create(stream, xws)) {
                    xmlDocument.Save(xw);
                }

            }

            Console.WriteLine($"Publish setting added in csproj!");
            return true;
        }

    }
}
