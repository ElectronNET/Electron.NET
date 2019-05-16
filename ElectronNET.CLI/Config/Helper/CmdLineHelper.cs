using System.Collections.Generic;

namespace ElectronNET.CLI.Config.Helper {

    /// <summary> Command line helper class. </summary>
    public class CmdLineHelper {

        /// <summary> Parses / Filters the arguments in the command line. </summary>
        /// <param name="opts"> Command line options. </param>
        /// <returns> A list of just the arguments without the switches. </returns>
        public static List<string> FilterArguments(IEnumerable<string> opts) {
            var ret = new List<string>();
            foreach (var item in opts) {
                var optitem = item;

                // For arguments we need to make sure the option does not start with -, --, /
                if ((optitem.StartsWith("-") || optitem.StartsWith("--") || optitem.StartsWith("/")))
                    continue;

                // remove double quotes from argument value
                optitem = optitem.Replace("\"", "");

                ret.Add(optitem);
            }
            return ret;
        }


        /// <summary> Parse the switches into a Dictionary. </summary>
        /// <param name="switches"> Command line switches. </param>
        /// <returns> Dictionary of parsed switches </returns>
        public static Dictionary<string, string> FilterSwitches(IEnumerable<string> switches) {
            var ret = new Dictionary<string, string>();
            foreach (var item in switches) {
                var switchitem = item;

                // Make sure the option starts with -, --, /
                if (!(switchitem.StartsWith("-") || switchitem.StartsWith("--") || switchitem.StartsWith("/")))
                    continue;

                // Remove the leading -, --, / from the front of the option for later parsing
                if (switchitem.StartsWith("--"))
                    switchitem = switchitem.ReplaceFirst("--", "");
                if (switchitem.StartsWith("-"))
                    switchitem = switchitem.ReplaceFirst("-", "");
                if (switchitem.StartsWith("/"))
                    switchitem = switchitem.ReplaceFirst("/", "");

                // Split based on the equals sign
                // remove double quotes from option value
                var swsplit = switchitem.Split("=");
                var key = swsplit[0];
                string value = null;
                if (swsplit.Length > 1) {
                    value = swsplit[1];
                    value = value.Replace("\"", "");
                }

                ret.Add(key, value);
            }
            return ret;
        }
    }
}
