using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ElectronNET.CLI.Commands.Actions;
using ElectronNET.CLI.Config.Helper;

namespace ElectronNET.CLI.Config.Commands {

    /// <summary> Configuration for the start command. </summary>
    public class StartConfig : ICommandConfig {

        /// <summary> Sets the full path to the ASP core project. </summary>
        /// <value> The full path of the ASP core project. </value>
        public string ProjectPath { get; set; }

        /// <summary> Path to the electron host hook files. </summary>
        /// <value> Path to the electron host hook files. </value>
        public string ElectronHostHookPath { get; set; }

        /// <summary> The package manager used to install packages. </summary>
        /// <value> which package manager to use. </value>
        public PackageManagerType NpmCommand { get; set; } = PackageManagerType.npm;

        /// <summary> Sets the directory to copy files to for running. </summary>
        /// <value> The full path of the run directory. </value>
        public string RunPath { get; set; }

        /// <summary> Desired target such as windows / linux / osx etc. </summary>
        /// <value> The desired target. </value>
        public DesiredPlatformInfo Target { get; set; }

        /// <summary> Override the runtime identifier used by dotnet publish. </summary>
        /// <value> The runtime identifier. </value>
        public string RuntimeIdentifier { get; set; }

        /// <summary> Additional options to pass to dotnet publish. </summary>
        /// <value> Additional command line options for dotnet publish. </value>
        public string DotnetAdditionalOpts { get; set; }

        /// <summary> Additional options to pass to electron. </summary>
        /// <value> Additional command line options for electron. </value>
        public string ElectronParams { get; set; }

        /// <summary> Force the node_modules directory to be reinstalled. </summary>
        /// <value> True if force npm install, false if not. </value>
        public bool ForceNpmInstall { get; set; }

        /// <summary> Parse Commandline options into key value pairs. </summary>
        /// <param name="data">     The existing configuration data - key / value pairs. </param>
        /// <param name="args">     The arguments. </param>
        /// <param name="switches"> The switches. </param>
        /// <returns> A Dictionary&lt;string,string&gt; </returns>
        public static Dictionary<string, string> ParseCommandline(
            Dictionary<string, string> data,
            List<string> args,
            Dictionary<string, string> switches) {

            // Overrides the source project path directory
            if (args.Count > 1 && args[0] != "help")
                data["start:projectpath"] = args[1];

            // Overrides the directory for the electron hosthook files
            if (switches.ContainsKey("hosthookpath"))
                data["add:hosthook:hosthookpath"] = switches["hosthookpath"];

            // Npm command to use
            if (switches.ContainsKey("npmcommand"))
                data["start:npmcommand"] = switches["npmcommand"];

            // Overrides the directory to use for running / destination
            if (switches.ContainsKey("runpath"))
                data["start:runpath"] = switches["runpath"];

            // Which desired platform to use win, linux, osxx, etc
            if (switches.ContainsKey("target"))
                data["start:target"] = switches["target"];

            // Overrides the runtime identifier for dotnet publish
            if (switches.ContainsKey("runtimeid"))
                data["start:runtimeid"] = switches["runtimeid"];

            // Additional options to pass to dotnet publish
            if (switches.ContainsKey("dotnetopts"))
                data["start:dotnetopts"] = switches["dotnetopts"];

            // Additional options to pass to electron
            if (switches.ContainsKey("electron-params"))
                data["start:electron-params"] = switches["electron-params"];

            // If to force npm to re-install the node_modules
            if (switches.ContainsKey("install-modules"))
                data["start:install-modules"] = "true";

            return data;
        }

        /// <summary> Binds the settings based on the builder. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Bind(IConfiguration builder) {

            // Global Application settings
            var setts = SettingsLoader.Settings;

            // Overrides the source project path directory
            ProjectPath = builder["start:projectpath"] ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(ProjectPath)) {
                Console.WriteLine("Unable to find directory");
                Console.WriteLine($"projectpath: {ProjectPath}");
                return false;
            }

            // Overrides the directory for the electron hosthook files
            ElectronHostHookPath = builder["add:hosthook:hosthookpath"] ?? Path.Combine(ProjectPath, "ElectronHostHook");

            // Specify which package manager to use npm, yarn, pnpm
            // pnpm is good at saving space on the disk
            try {
                var npmcmdstr = builder["start:npmcommand"] ?? "npm";
                NpmCommand = EnumHelper.Parse<PackageManagerType>(npmcmdstr, "npmcommand");
            }
            catch (ArgumentException ex) {
                if (!setts.ShowHelp) Console.WriteLine(ex.Message);
                setts.ShowHelp = true;
                return false;
            }

            // Overrides the directory to use for running / destination
            RunPath = builder["start:runpath"];

            // Which desired platform to use win, linux, osxx, etc
            try {
                var desiredplat = builder["start:target"] ?? "auto";
                Target = EnumHelper.Parse<DesiredPlatformInfo>(desiredplat, "target");
            }
            catch (ArgumentException ex) {
                if (!setts.ShowHelp) Console.WriteLine(ex.Message);
                setts.ShowHelp = true;
                return false;
            }

            // Overrides the runtime identifier for dotnet publish
            RuntimeIdentifier = builder["start:runtimeid"] ?? Target.ToNetCorePublishRid();

            // Additional options to pass to dotnet publish
            DotnetAdditionalOpts = builder["start:dotnetopts"];

            // Additional options to pass to electron
            ElectronParams = builder["start:electron-params"];

            // If to force npm to re-install the node_modules
            if (builder["start:install-modules"] == "true")
                ForceNpmInstall = true;

            return true;
        }


        public const string cmd_name = "start";
        public const string cmd_description = "Start the ASP.NET Core Application with Electron.";

        /// <summary> Print the Usage. </summary>
        public void PrintUsage() {
            var helptxt = new StringBuilder();

            // {Column number, width}, if width is negative then left align
            const string strfmt = "    {0,-26} {1,-10}";

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
            helptxt.AppendFormat(strfmt, "--npmcommand=<value>", "Which package manager to use\n");
            helptxt.AppendFormat(strfmt, "", $"(valid values: {EnumHelper.CommaValues<PackageManagerType>()})\n");
            helptxt.AppendFormat(strfmt, "", "(default: npm)\n");
            helptxt.AppendFormat(strfmt, "--runpath=<Path>", "Destination directory for running\n");
            helptxt.AppendFormat(strfmt, "", "(default: bin/Host)\n");
            helptxt.AppendFormat(strfmt, "--target=<value>", "Specify the desired target\n");
            helptxt.AppendFormat(strfmt, "", $"(valid values: {EnumHelper.CommaValues<DesiredPlatformInfo>()})\n");
            helptxt.AppendFormat(strfmt, "", "(default: auto)\n");
            helptxt.AppendFormat(strfmt, "--runtimeid=<value>", "Runtime identifier for dotnet publish\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to the value from desiredplatform)\n");
            helptxt.AppendFormat(strfmt, "--hosthookpath=<Path>", "directory for the electron host hook files\n");
            helptxt.AppendFormat(strfmt, "", "(default: <ProjectPath>/ElectronHostHook)\n");
            helptxt.AppendFormat(strfmt, "--dotnetopts=<value>", "Additional command line options to pass to dotnet publish\n");
            helptxt.AppendFormat(strfmt, "--electron-params=<value>", "Additional parameters to pass to electron\n");
            helptxt.AppendFormat(strfmt, "--install-modules", "Force a re-install of node_modules\n");
            Console.Write(helptxt.ToString());
        }
    }
}
