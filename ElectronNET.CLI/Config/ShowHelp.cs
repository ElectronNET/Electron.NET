using ElectronNET.CLI.Config.Commands;
using System;
using System.Text;

namespace ElectronNET.CLI.Config {

    /// <summary> Shows the help. </summary>
    public class ShowHelp {

        /// <summary> Shows the help. </summary>
        public static void Show() {
            var setts = SettingsLoader.Settings;
            if (setts?.CommandName == null) {
                PrintUsage();
            }
            else {
                if (setts.CommandConfig != null) {
                    setts.CommandConfig.PrintUsage();
                }
                else {
                    PrintUsage();
                }
            }
        }

        /// <summary> Print usage - General help. </summary>
        private static void PrintUsage() {
            var helptxt = new StringBuilder();

            // {Column number, width}, if width is negative then left align
            const string strfmt = "    {0,-12} {1,-10}";

            PrintHeader();
            helptxt.AppendLine("These are the electronize commands available:");
            helptxt.AppendLine("");
            helptxt.AppendFormat(strfmt, InitConfig.cmd_name, InitConfig.cmd_description + "\n");
            helptxt.AppendFormat(strfmt, BuildConfig.cmd_name, BuildConfig.cmd_description + "\n");
            helptxt.AppendFormat(strfmt, StartConfig.cmd_name, StartConfig.cmd_description + "\n");
            helptxt.AppendFormat(strfmt, AddConfig.cmd_name, AddConfig.cmd_description + "\n");
            helptxt.AppendFormat(strfmt, "version", "Show the version of electronize\n");
            helptxt.AppendFormat(strfmt, "help", "Show Help\n");
            helptxt.AppendLine("");
            helptxt.AppendLine("To get help on individual commands execute:");
            helptxt.AppendFormat(strfmt, "electronize help <command>", "\n");
            Console.Write(helptxt.ToString());
        }

        /// <summary> Print usage - Header. </summary>
        public static void PrintHeader() {
            Console.WriteLine("usage: electronize [--version] [--help] <command> [<commandopts>]");
            Console.WriteLine("\t");
        }

    }
}
