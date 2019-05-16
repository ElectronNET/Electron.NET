using ElectronNET.CLI.Commands;
using ElectronNET.CLI.Config.Commands;
using ElectronNET.CLI.Config.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectronNET.CLI.Config {

    /// <summary> The application settings. </summary>
    public class AppSettings {

        /// <summary> If to show help. </summary>
        /// <value> True if show help, false if not. </value>
        public bool ShowHelp { get; set; }

        /// <summary> Path to the configuration file. </summary>
        /// <value> The full pathname of the configuration file. </value>
        public string ConfigFilePath { get; set; }

        /// <summary> Specifies which command is in use. </summary>
        /// <value> The name of the command. </value>
        public CommandType? CommandName { get; set; }

        /// <summary> Configuration settings class for the given command. </summary>
        /// <value> The command configuration class. </value>
        public ICommandConfig CommandConfig { get; set; }


        /// <summary> Loads the given options into key value pairs. </summary>
        /// <param name="opts"> The Options to load. </param>
        /// <returns> The options in key / value pairs </returns>
        public static Dictionary<string, string> Load(string[] opts) {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // If no options show help
            if (!opts.Any()) {
                data["cmdline:showhelp"] = "true";
                return data;
            }

            // Get the arguments / switches
            var args = CmdLineHelper.FilterArguments(opts);
            var switches = CmdLineHelper.FilterSwitches(opts);

            // Shortcuts to --help and --version
            if (switches.ContainsKey("help"))
                data["cmdline:showhelp"] = "true";
            
            if (switches.ContainsKey("version")) {
                data["cmdline:cmdname"] = "version";
                return data;
            }

            // Assume the first argument is the command name unless help is asked for
            if (args.Count > 0) {
                if (args.First() == "help") {
                    data["cmdline:showhelp"] = "true";
                    // Help for a command
                    if (args.Count > 1)
                        data["cmdline:cmdname"] = opts.ElementAt(1);
                }
                else {
                    // Command is first argument
                    data["cmdline:cmdname"] = args.First();
                }
            }

            // Read configuration from a different json file
            if (switches.ContainsKey("cfgfile"))
                data["general:cfgfile"] = switches["cfgfile"];

            // Command specific opts
            if (data.ContainsKey("cmdline:cmdname")) {
                if (data["cmdline:cmdname"] == "start")
                    data = StartConfig.ParseCommandline(data, args, switches);
                else if (data["cmdline:cmdname"] == "init")
                    data = InitConfig.ParseCommandline(data, args, switches);
                else if (data["cmdline:cmdname"] == "build")
                    data = BuildConfig.ParseCommandline(data, args, switches);
                else if (data["cmdline:cmdname"] == "add")
                    data = AddConfig.ParseCommandline(data, args, switches);
            }
            return data;
        }


        /// <summary> Binds the settings. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Bind(IConfiguration builder) {

            // Any of these settings can be sourced via the command line or a configuration file
            // However those prefixed with cmdline: are typically reserved for useage from the command line only

            // Config file path
            ConfigFilePath = builder["cmdline:cfgfile"];

            // Show help
            if (builder["cmdline:showhelp"] == "true")
                ShowHelp = true;

            // Parse the command name
            try {
                CommandName = EnumHelper.Parse<CommandType>(builder["cmdline:cmdname"], "Command name");
            }
            catch (ArgumentException ex) {
                if (!ShowHelp) Console.WriteLine(ex.Message);
                ShowHelp = true;
                return false;
            }

            // Setup a command specific config
            CommandConfig = CommandName.NewConfig();
            if (CommandConfig != null && !CommandConfig.Bind(builder))
                return false;

            return true;
        }
    }
}
