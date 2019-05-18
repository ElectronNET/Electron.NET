using ElectronNET.CLI.Commands.Actions;
using ElectronNET.CLI.Config.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectronNET.CLI.Config.Commands {

    /// <summary> Configuration for the build command. </summary>
    public class BuildConfig : ICommandConfig {

        /// <summary> Sets the full path to the ASP core project. </summary>
        /// <value> The full path of the ASP core project. </value>
        public string ProjectPath { get; set; }

        /// <summary> Path to the electron host hook files. </summary>
        /// <value> Path to the electron host hook files. </value>
        public string ElectronHostHookPath { get; set; }
        public bool HookpathChanged { get; set; }

        /// <summary> The package manager used to install packages. </summary>
        /// <value> which package manager to use. </value>
        public PackageManagerType NpmCommand { get; set; } = PackageManagerType.npm;

        /// <summary> Temporary build directory for intermediary files. </summary>
        /// <value> Temporary build destination path. </value>
        public string TmpBuildPath { get; set; }

        /// <summary> The dotnet configuration - Debug / Release. </summary>
        /// <value> dotnet configuration. </value>
        public string DotnetConfiguration { get; set; }

        /// <summary> Desired target such as windows / linux / osx etc. </summary>
        /// <value> The desired target. </value>
        public DesiredPlatformInfo Target { get; set; }

        /// <summary> Override the runtime identifier used by dotnet publish. </summary>
        /// <value> The runtime identifier. </value>
        public string RuntimeIdentifier { get; set; }

        /// <summary> Override the electron packer platform setting. </summary>
        /// <value> The electron packer platform. </value>
        public string ElectronPackerPlatform { get; set; }

        /// <summary> The setting to pass to electron for the arch. </summary>
        /// <value> The electron arch setting. </value>
        public string ElectronArch { get; set; }

        /// <summary> Additional parameters to pass to electron. </summary>
        /// <value> Additional parameters to pass to electron. </value>
        public string ElectronParams { get; set; }

        /// <summary> Force the node_modules directory to be reinstalled. </summary>
        /// <value> True if force npm install, false if not. </value>
        public bool ForceNpmInstall { get; set; }

        /// <summary> If to specify a package json file. </summary>
        /// <value> Path to a package JSON file. </value>
        public string PackageJsonFile { get; set; }


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
                data["build:projectpath"] = args[1];

            // Overrides the destination directory for the electron hosthook files
            if (switches.ContainsKey("hosthookpath"))
                data["add:hosthook:hosthookpath"] = switches["hosthookpath"];

            // Npm command to use
            if (switches.ContainsKey("npmcommand"))
                data["build:npmcommand"] = switches["npmcommand"];

            // Overrides the dotnet configuration - Debug / Release
            if (switches.ContainsKey("dotnet-configuration"))
                data["build:dotnet-configuration"] = switches["dotnet-configuration"];

            // Which desired platform to use win, linux, osxx, etc
            if (switches.ContainsKey("target"))
                data["build:target"] = switches["target"];

            // Overrides the runtime identifier for dotnet publish
            if (switches.ContainsKey("runtimeid"))
                data["build:runtimeid"] = switches["runtimeid"];

            // Overrides the electron packer platform
            if (switches.ContainsKey("electronpacker"))
                data["build:electronpacker"] = switches["electronpacker"];

            // electron arch setting
            if (switches.ContainsKey("electron-arch"))
                data["build:electron-arch"] = switches["electron-arch"];

            // Additional electron parameters
            if (switches.ContainsKey("electron-params"))
                data["build:electron-params"] = switches["electron-params"];

            // If to force npm to re-install the node_modules
            if (switches.ContainsKey("install-modules"))
                data["build:install-modules"] = "true";

            // Specify a package json file
            if (switches.ContainsKey("package-json"))
                data["build:package-json"] = switches["package-json"];

            return data;
        }

        /// <summary> Binds the settings based on the builder. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Bind(IConfiguration builder) {

            // Global Application settings
            var setts = SettingsLoader.Settings;

            // Overrides the source project path directory
            ProjectPath = builder["build:projectpath"] ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(ProjectPath)) {
                Console.WriteLine("Unable to find directory");
                Console.WriteLine($"projectpath: {ProjectPath}");
                return false;
            }

            // Overrides the destination directory for the electron hosthook files
            if (builder["add:hosthook:hosthookpath"] != null)
                HookpathChanged = true;
            ElectronHostHookPath = builder["add:hosthook:hosthookpath"] ?? Path.Combine(ProjectPath, "ElectronHostHook");

            // Specify which package manager to use npm, yarn, pnpm
            // pnpm is good at saving space on the disk
            try {
                var npmcmdstr = builder["build:npmcommand"] ?? "npm";
                NpmCommand = EnumHelper.Parse<PackageManagerType>(npmcmdstr, "npmcommand");
            }
            catch (ArgumentException ex) {
                if (!setts.ShowHelp) Console.WriteLine(ex.Message);
                setts.ShowHelp = true;
                return false;
            }

            // Overrides the dotnet configuration - Debug / Release
            DotnetConfiguration = builder["build:dotnet-configuration"] ?? "Release";

            // Which desired platform to use win, linux, osxx, etc
            try {
                var desiredplat = builder["build:target"] ?? "auto";
                Target = EnumHelper.Parse<DesiredPlatformInfo>(desiredplat, "target");
            }
            catch (ArgumentException ex) {
                if (!setts.ShowHelp) Console.WriteLine(ex.Message);
                setts.ShowHelp = true;
                return false;
            }

            // Temp build directory
            TmpBuildPath = Path.Combine(ProjectPath, "obj", "desktop", Target.ToString());
            if (!Directory.Exists(TmpBuildPath))
                Directory.CreateDirectory(TmpBuildPath);

            // Overrides the runtime identifier for dotnet publish
            RuntimeIdentifier = builder["build:runtimeid"] ?? Target.ToNetCorePublishRid();

            // Overrides the electron packer platform
            ElectronPackerPlatform = builder["build:electronpacker"] ?? Target.ToElectronPackerPlatform();

            // electron arch setting
            ElectronArch = builder["build:electron-arch"] ?? Target.ToElectronArch();

            // Additional electron parameters
            ElectronParams = builder["build:electron-params"];

            // If to force npm to re-install the node_modules
            if (builder["build:install-modules"] == "true")
                ForceNpmInstall = true;

            // Specify a package json file
            PackageJsonFile = builder["build:package-json"];

            return true;
        }



        public const string cmd_name = "build";
        public const string cmd_description = "Build the Electron Application.";

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
            helptxt.AppendFormat(strfmt, "<Project Path>", "Source project directory\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to current directory)\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Options:");
            helptxt.AppendFormat(strfmt, "--hosthookpath=<Path>", "Destination directory for the electron host hook files\n");
            helptxt.AppendFormat(strfmt, "", "(default: <ProjectPath>/ElectronHostHook)\n");
            helptxt.AppendFormat(strfmt, "--npmcommand=<value>", "Which package manager to use\n");
            helptxt.AppendFormat(strfmt, "", $"(valid values: {EnumHelper.CommaValues<PackageManagerType>()})\n");
            helptxt.AppendFormat(strfmt, "", "(default: npm)\n");
            helptxt.AppendFormat(strfmt, "--dotnet-configuration=<config>", "Specify the dotnet configuration to use\n");
            helptxt.AppendFormat(strfmt, "", "(default: Release)\n");
            helptxt.AppendFormat(strfmt, "--target=<value>", "Specify the desired target\n");
            helptxt.AppendFormat(strfmt, "", $"(valid values: {EnumHelper.CommaValues<DesiredPlatformInfo>()})\n");
            helptxt.AppendFormat(strfmt, "", "(default: auto)\n");
            helptxt.AppendFormat(strfmt, "--runtimeid=<value>", "Runtime identifier for dotnet publish, e.g. \"win7-x86\"\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to the value from desiredplatform) see the .NET Core RID Catalog\n");
            helptxt.AppendFormat(strfmt, "--electronpacker=<value>", "Specify the resulting electron processor architecture\n");
            helptxt.AppendFormat(strfmt, "", "(defaults to the value from desiredplatform) e.g. win32 or ia86 for x86 builds\n");
            helptxt.AppendFormat(strfmt, "--electron-arch=<value>", "Electron arch setting to use\n");
            helptxt.AppendFormat(strfmt, "", "(default: x64)\n");
            helptxt.AppendFormat(strfmt, "--electron-params=<value>", "Additional parameters to pass to the electron packager\n");
            helptxt.AppendFormat(strfmt, "--install-modules", "Force install of node_modules, Implied by '--package-json'\n");
            helptxt.AppendFormat(strfmt, "--package-json=<FilePath>", "Specify a custom package json file\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("  Full example for a 32bit debug build with electron prune:");
            helptxt.AppendLine("    electronize build --runtimeid=\"win7-x86\" --electronpacker=\"win32\" --dotnet-configuration=Debug");
            helptxt.AppendLine("    --electron-arch=ia32  --electron-params=\"--prune=true\"");
            Console.Write(helptxt.ToString());
        }
    }
}
