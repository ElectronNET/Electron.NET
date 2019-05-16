using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectronNET.CLI.Config.Commands {

    /// <summary> Configuration for the init command. </summary>
    public class InitConfig : ICommandConfig {

        /// <summary> Overrides the full path to the ASP core project. </summary>
        /// <value> The full path of the ASP core project. </value>
        public string ProjectPath { get; set; }

        /// <summary> Overrides the electron manifest file name. </summary>
        /// <value> The electron manifest file name. </value>
        public string ElectronManifestFile { get; set; }

        /// <summary> Overrides the project file to use. </summary>
        /// <value> The project file to use. </value>
        public string ProjectFile { get; set; }

        /// <summary> Overrides the launch settings file to use. </summary>
        /// <value> The launch settings file to use. </value>
        public string LaunchSettingsFile { get; set; }


        /// <summary> Parse Command line options into key value pairs. </summary>
        /// <param name="data">     The existing configuration data - key / value pairs. </param>
        /// <param name="args">     The arguments. </param>
        /// <param name="switches"> The switches. </param>
        /// <returns> A Dictionary&lt;string,string&gt; </returns>
        public static Dictionary<string, string> ParseCommandline(
            Dictionary<string, string> data,
            List<string> args,
            Dictionary<string, string> switches) {

            // Overrides the source project path directory
            if (args.Count > 1 && args[0] != "help") {
                data["init:projectpath"] = args[1];
            }

            // Overrides the filename of the electron manifest
            if (switches.ContainsKey("electronmanifest"))
                data["init:electronmanifest"] = switches["electronmanifest"];

            // Overrides the project file to use
            if (switches.ContainsKey("projectfile"))
                data["init:projectfile"] = switches["projectfile"];

            // Overrides the launch settings file to use
            if (switches.ContainsKey("launchsettingsfile"))
                data["init:launchsettingsfile"] = switches["launchsettingsfile"];

            return data;
        }

        /// <summary> Binds the settings based on the builder. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Bind(IConfiguration builder) {

            // Overrides the source project path directory
            ProjectPath = builder["init:projectpath"] ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(ProjectPath)) {
                Console.WriteLine("Unable to find directory");
                Console.WriteLine($"projectpath: {ProjectPath}");
                return false;
            }

            // Overrides the filename of the electron manifest
            ElectronManifestFile = builder["init:electronmanifest"] ?? "electron.manifest.json";

            // Overrides the project file to use
            ProjectFile = builder["init:projectfile"];

            // Overrides the project file to use
            LaunchSettingsFile = builder["init:launchsettingsfile"];

            return true;
        }



        public const string cmd_name = "init";
        public const string cmd_description = "Creates the needed Electron.NET config for the Electron Application.";

        /// <summary> Print the Usage. </summary>
        public void PrintUsage() {
            var helptxt = new StringBuilder();

            // {Column number, width}, if width is negative then left align
            const string strfmt = "    {0,-32} {1,-10}";

            ShowHelp.PrintHeader();
            helptxt.AppendLine($"{cmd_name}:");
            helptxt.AppendLine($"  {cmd_description}");
            helptxt.AppendLine("");
            helptxt.AppendLine($"  electronize {cmd_name} [arguments] [options]");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Arguments:");
            helptxt.AppendFormat(strfmt, "<Path>", "Source project directory\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to current directory)\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Options:");
            helptxt.AppendFormat(strfmt, "--projectfile=<Filepath>", "Specify the path to the project file\n");
            helptxt.AppendFormat(strfmt, "", "(default: searches projectpath for *.csproj)\n");
            helptxt.AppendFormat(strfmt, "--electronmanifest=<Filename>", "Specify file name of the electron manifest file\n");
            helptxt.AppendFormat(strfmt, "", "(default: electron.manifest.json)\n");
            helptxt.AppendFormat(strfmt, "--launchsettingsfile=<Filename>", "Specify file name of the launch settings file\n");
            helptxt.AppendFormat(strfmt, "", "(default: Properties/launchSettings.json)\n");
            Console.Write(helptxt.ToString());
        }
    }
}
