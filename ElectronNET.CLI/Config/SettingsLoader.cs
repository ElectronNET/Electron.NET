using ElectronNET.CLI.Config.CmdLineProvider;
using ElectronNET.CLI.Config.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace ElectronNET.CLI.Config {

    /// <summary> The settings loader. </summary>
    public class SettingsLoader {

        /// <summary> Global application configuration. </summary>
        /// <value> Global application configuration. </value>
        public static IConfiguration Builder { get; set; }

        /// <summary> Global application configuration. </summary>
        /// <value> Global application configuration. </value>
        public static AppSettings Settings { get; set; }

        /// <summary> Loads the settings from the command line / configuration file. </summary>
        /// <param name="args"> Command line arguments. </param>
        public static bool Load(string[] args) {

            // This uses the .Net Core configuration framework (Microsoft library)
            // 1. first it looks for a parameter called cfgfile to point to a configuration file
            // 2. If not found it defaults to looking for electron-net/ElectronNetSettings.json
            // Although this is optional
            // 
            // Next we do the actual loading of the configuration
            // 1. Configuration is loaded from a json file if it exists (optional)
            // 2. Configuration is overridden by command line options
            //    We use our own Command line provider instead of Microsoft's for handling commands
            // 3. At this stage the configuration is a series of key value pairs (Dictionary)
            // 4. These key / value pairs are then bound to an AppSettings class instance
            //    and a Command Configuration class instance (such as Initconfig)

            // For the json configuration file a setting such as "init:projectpath" could be set via
            // {
            //   "init": {
            //     "projectpath": "some path to a directory",
            //   }
            // }


            // Normally the config file is read in first then any command line options override it
            // We do an initial read first just to get the config file path
            var cfgfile = GetCfgFilePath(args);
            if (cfgfile == null) {
                // Default path for configuration file - optional
                cfgfile = "ElectronNetSettings.json";
            }
            else {
                // If the user has specified a different configuration file then make sure it exists
                if (!File.Exists(cfgfile)) {
                    Console.WriteLine($"Error unable to find configuration file: {cfgfile}");
                    return false;
                }
            }



            // Read in the Json config file first, then override with arguments from the command line
            Builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(cfgfile, optional: true, reloadOnChange: true)
                .AddCmdLineDirectCallback(args, AppSettings.Load)
                .Build();
            // Make a note for later use if needed
            Builder["cmdline:cfgfile"] = cfgfile;

            Settings = new AppSettings();
            return Settings.Bind(Builder);
        }


        /// <summary> Determine if someone has specified an alternative configuration file. </summary>
        /// <param name="opts"> Command line options. </param>
        /// <returns> The configuration file path. </returns>
        private static string GetCfgFilePath(IEnumerable<string> opts) {
            string cfgfile = null;
            // search the switches for a cfgfile setting i.e. --cfgfile="Settings2.json"
            var switches = CmdLineHelper.FilterSwitches(opts);
            if (switches.ContainsKey("cfgfile"))
                cfgfile = switches["cfgfile"];
            return cfgfile;
        }
    }
}
