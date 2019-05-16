using ElectronNET.CLI.Commands;
using ElectronNET.CLI.Config;
using System;

namespace ElectronNET.CLI {

    /// <summary> Main program. </summary>
    public class Program {

        /// <summary> Main entry-point for this application. </summary>
        /// <param name="args"> An array of command-line argument strings. </param>
        static void Main(string[] args) {

            // If there are no arguments show help
            if (args.Length == 0) {
                ShowHelp.Show();
                Environment.Exit(-1);
            }

            // If there was a problem parsing the settings show help
            if (!SettingsLoader.Load(args)) {
                ShowHelp.Show();
                Environment.Exit(-1);
            }

            // If help has been requested generally or for a specific command
            var setts = SettingsLoader.Settings;
            if (setts.ShowHelp) {
                ShowHelp.Show();
                Environment.Exit(0);
            }

            // Run the command
            var cmd = setts.CommandName.ToCmdObject();
            var success = cmd.ExecuteAsync().Result;
            if (!success) {
                Environment.Exit(-1);
            }
            Environment.Exit(0);
        }
    }
}
