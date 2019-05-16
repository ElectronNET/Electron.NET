using ElectronNET.CLI.Config;
using ElectronNET.CLI.Config.Commands;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ElectronNET.CLI.Commands {

    /// <summary> Initialize command. </summary>
    public class InitCommand : ICommand {

        /// <summary> General Application Settings. </summary>
        /// <value> General Application Settings. </value>
        private AppSettings appcfg { get; set; }

        /// <summary> Command specific settings. </summary>
        /// <value> Command specific settings. </value>
        private InitConfig cmdcfg { get; set; }

        /// <summary> Initialize Command Execute. </summary>
        /// <returns> Initialize Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {
                Console.WriteLine("Init Electron Desktop Application...");

                // Read in the configuration
                appcfg = SettingsLoader.Settings;
                cmdcfg = (InitConfig) appcfg.CommandConfig;

                // Find the .csproj file
                if (cmdcfg.ProjectFile == null) {
                    Console.WriteLine("Searching for Project file");
                    cmdcfg.ProjectFile = Directory
                        .EnumerateFiles(cmdcfg.ProjectPath, "*.csproj",
                            SearchOption.TopDirectoryOnly).FirstOrDefault();
                }
                if (cmdcfg.ProjectFile == null || !File.Exists(cmdcfg.ProjectFile)) {
                    Console.WriteLine("Error unable to locate .csproj file");
                    return false;
                }
                Console.WriteLine($"Project file found: {cmdcfg.ProjectFile}");

                // Add electron manifest file
                AddManifest();

                // Edit the csproj file if needed
                if (!EditCsProj())
                    return false;

                // Edit the launchSettings.json if needed
                if (!EditLaunchSettings())
                    return false;

                Console.WriteLine("Everything done - happy electronizing!");
                return true;
            });
        }


        /// <summary> Adds the electron manifest file to the project. </summary>
        private void AddManifest() {
            Console.WriteLine("Adding the electron manifest file to your project...");

            var targetFilePath = Path.Combine(cmdcfg.ProjectPath, "electron.manifest.json");
            if (File.Exists(targetFilePath)) {
                Console.WriteLine("electron manifest file already found: electron.manifest.json");
                Console.WriteLine("Skipping");
                return;
            }

            // Deploy config file
            EmbeddedFileHelper.DeployEmbeddedFile(cmdcfg.ProjectPath, "electron.manifest.json");

            // update config file with the name of the csproj
            // ToDo: If the csproj name != application name, this will fail
            Console.WriteLine($"Updating manifest with name of project: {Path.GetFileName(cmdcfg.ProjectFile)}");
            var text = File.ReadAllText(targetFilePath);
            text = text.Replace("{{executable}}", Path.GetFileNameWithoutExtension(cmdcfg.ProjectFile));
            File.WriteAllText(targetFilePath, text);
        }


        /// <summary> Edit the .csproj project file </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool EditCsProj() {
            Console.WriteLine("Checking to see if we need to update the .csproj project file");
            using (var stream = File.Open(cmdcfg.ProjectFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                var xmlDocument = XDocument.Load(stream);

                var projectElement = xmlDocument.Descendants("Project").FirstOrDefault();
                if (projectElement == null || projectElement.Attribute("Sdk")?.Value != "Microsoft.NET.Sdk.Web") {
                    Console.WriteLine(
                        $"Project file is not a compatible type of 'Microsoft.NET.Sdk.Web'. Your project: {projectElement?.Attribute("Sdk")?.Value}");
                    return false;
                }

                if (xmlDocument.ToString().Contains("Content Update=\"electron.manifest.json\"")) {
                    Console.WriteLine("electron.manifest.json already in csproj.");
                    return true;
                }

                Console.WriteLine("electron.manifest.json will be added to csproj.");

                var itemGroupXmlString = "<ItemGroup>" +
                                         "<Content Update=\"electron.manifest.json\">" +
                                         "<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" +
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

            Console.WriteLine("electron.manifest.json added in csproj!");
            return true;
        }


        /// <summary> Edit the launch settings file. </summary>
        private bool EditLaunchSettings() {
            // super stupid implementation, but because there is no nativ way to parse json
            // and cli extensions and other nuget packages are buggy 
            // this is should solve the problem for 80% of the users
            // for the other 20% we might fail... 

            if (cmdcfg.LaunchSettingsFile == null) {
                Console.WriteLine("Searching for launchSettings.json to add our electron debug profile...");
                cmdcfg.LaunchSettingsFile = Path.Combine(cmdcfg.ProjectPath, "Properties", "launchSettings.json");
            }

            if (File.Exists(cmdcfg.LaunchSettingsFile) == false) {
                Console.WriteLine("Error unable to locate launch settings config file");
                return false;
            }

            var launchSettingText = File.ReadAllText(cmdcfg.LaunchSettingsFile);

            if (launchSettingText.Contains("\"executablePath\": \"electronize\"") == false) {
                var debugProfileBuilder = new StringBuilder();
                debugProfileBuilder.AppendLine("profiles\": {");
                debugProfileBuilder.AppendLine("    \"Electron.NET App\": {");
                debugProfileBuilder.AppendLine("      \"commandName\": \"Executable\",");
                debugProfileBuilder.AppendLine("      \"executablePath\": \"electronize\",");
                debugProfileBuilder.AppendLine("      \"commandLineArgs\": \"start\",");
                debugProfileBuilder.AppendLine("      \"workingDirectory\": \".\"");
                debugProfileBuilder.AppendLine("    },");

                launchSettingText = launchSettingText.Replace("profiles\": {", debugProfileBuilder.ToString());
                File.WriteAllText(cmdcfg.LaunchSettingsFile, launchSettingText);

                Console.WriteLine("Debug profile added!");
            }
            else {
                Console.WriteLine("Debug profile already exists Skipping");
            }
            return true;
        }
    }
}
