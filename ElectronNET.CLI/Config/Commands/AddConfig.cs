using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ElectronNET.CLI.Config.Helper;

namespace ElectronNET.CLI.Config.Commands {

    /// <summary> Configuration for the add command. </summary>
    public class AddConfig : ICommandConfig {

        /// <summary> Gets or sets the SubCommand. </summary>
        /// <value> The SubCommand. </value>
        public string SubCommand { get; set; }

        /// <summary> Sets the full path to the ASP core project. </summary>
        /// <value> The full path of the ASP core project. </value>
        public string ProjectPath { get; set; }

        /// <summary> Path to the electron host hook files. </summary>
        /// <value> Path to the electron host hook files. </value>
        public string ElectronHostHookPath { get; set; }

        /// <summary> The package manager used to install packages. </summary>
        /// <value> which package manager to use. </value>
        public PackageManagerType NpmCommand { get; set; } = PackageManagerType.npm;

        /// <summary> Overrides the project file to use. </summary>
        /// <value> The project file to use. </value>
        public string ProjectFile { get; set; }


        /// <summary> Parse Commandline options into key value pairs. </summary>
        /// <param name="data">     The existing configuration data - key / value pairs. </param>
        /// <param name="args">     The arguments. </param>
        /// <param name="switches"> The switches. </param>
        /// <returns> A Dictionary&lt;string,string&gt; </returns>
        public static Dictionary<string, string> ParseCommandline(
            Dictionary<string, string> data,
            List<string> args,
            Dictionary<string, string> switches) {

            // Specify the sub command
            if (args.Count > 1 && args[0] != "help") {
                data["add:subcommand"] = args[1].ToLowerInvariant();
            }

            // Overrides the source project path directory, second argument
            if (args.Count > 2 && args[0] != "help") {
                data["add:projectpath"] = args[2];
            }

            // Overrides the destination directory for the electron hosthook files
            if (switches.ContainsKey("electronhosthookpath"))
                data["add:hosthook:hosthookpath"] = switches["electronhosthookpath"];

            // Npm command to use
            if (switches.ContainsKey("npmcommand"))
                data["add:npmcommand"] = switches["npmcommand"];

            // Overrides the project file to use
            if (switches.ContainsKey("projectfile"))
                data["add:projectfile"] = switches["projectfile"];

            return data;
        }

        /// <summary> Binds the settings based on the builder. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Bind(IConfiguration builder) {

            // Global Application settings
            var setts = SettingsLoader.Settings;

            // Read in the sub command
            SubCommand = builder["add:subcommand"];
            if (SubCommand != "hosthook") {
                Console.WriteLine($"Invalid sub command: {SubCommand}");
                return false;
            }

            // Overrides the source project path directory
            ProjectPath = builder["add:projectpath"] ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(ProjectPath)) {
                Console.WriteLine("Unable to find directory");
                Console.WriteLine($"projectpath: {ProjectPath}");
                return false;
            }

            // Overrides the destination directory for the electron hosthook files
            ElectronHostHookPath = builder["add:hosthook:hosthookpath"] ?? Path.Combine(ProjectPath, "ElectronHostHook");

            // Specify which package manager to use npm, yarn, pnpm
            // pnpm is good at saving space on the disk
            try {
                var npmcmdstr = builder["add:npmcommand"] ?? "npm";
                NpmCommand = EnumHelper.Parse<PackageManagerType>(npmcmdstr, "npmcommand");
            }
            catch (ArgumentException ex) {
                if (!setts.ShowHelp) Console.WriteLine(ex.Message);
                setts.ShowHelp = true;
                return false;
            }

            // Overrides the project file to use
            ProjectFile = builder["init:projectfile"];

            return true;
        }


        public const string cmd_name = "add";
        public const string cmd_description = "Command to add a custom npm packages to the Electron Application:";

        /// <summary> Print the Usage. </summary>
        public void PrintUsage() {
            var helptxt = new StringBuilder();

            // {Column number, width}, if width is negative then left align
            const string strfmt = "    {0,-26} {1,-10}";

            ShowHelp.PrintHeader();
            helptxt.AppendLine($"{cmd_name}:");
            helptxt.AppendLine($"  {cmd_description}");
            helptxt.AppendLine("");
            helptxt.AppendLine($"  electronize {cmd_name} <sub command> [arguments] [options]");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Sub Commands:");
            helptxt.AppendFormat(strfmt, "hosthook", "This creates a special folder for your custom npm package installation.\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Arguments:");
            helptxt.AppendFormat(strfmt, "<Path>", "Source project directory\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to current directory)\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Options:");
            helptxt.AppendFormat(strfmt, "--npmcommand=npm,yarn,pnpm", "Which package manager to use\n");
            helptxt.AppendFormat(strfmt, "", "(default: npm)\n");
            helptxt.AppendFormat(strfmt, "--electronhosthookpath=<Path>", "Destination directory for the electron host hook files\n");
            helptxt.AppendFormat(strfmt, "", "(default: <ProjectPath>/ElectronHostHook)\n");
            helptxt.AppendFormat(strfmt, "--projectfile=<Filepath>", "Specify the path to the project file\n");
            helptxt.AppendFormat(strfmt, "", "(default: searches projectpath for *.csproj)\n");
            Console.Write(helptxt.ToString());
        }
    }
}
