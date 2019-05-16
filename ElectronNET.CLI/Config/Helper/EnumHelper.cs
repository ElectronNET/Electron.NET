using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectronNET.CLI.Config.Helper {

    /// <summary> Enum Helper class. </summary>
    public static class EnumHelper {

        /// <summary> Parses a string into an enum. </summary>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
        ///                                      illegal values. </exception>
        /// <typeparam name="T"> Enum type. </typeparam>
        /// <param name="enum_name">       Name of the enum as a string. </param>
        /// <param name="param_label">     (Optional) The parameter label. </param>
        /// <param name="case_sensitivity"> (Optional) The case sensitivity. </param>
        /// <returns> A T. </returns>
        public static T Parse<T>(string enum_name, string param_label = "parameter",
            IEqualityComparer<string> case_sensitivity = null) where T : struct {
            // Get the list of enum names
            var names = Enum.GetNames(typeof(T)).ToList();

            // Check for the case sensitivity setting
            if (case_sensitivity == null)
                case_sensitivity = StringComparer.OrdinalIgnoreCase;
            
            // Check if the provided name is in the list, otherwise throw out a custom error for the help output.
            if (!names.Contains(enum_name, case_sensitivity)) {
                var exstr = $"Invalid {param_label}: {enum_name}\n";
                exstr += (names.Aggregate($"{param_label} can only be: ", (current, item) => current + item + ", "));
                throw new ArgumentException(exstr);
            }

            // Convert string to enum
            Enum.TryParse(enum_name, false, out T ret);
            return ret;
        }

        /// <summary> Get a comma separated list of enum values. </summary>
        /// <typeparam name="T"> Enum type. </typeparam>
        /// <returns> comma separated list of enum values. </returns>
        public static string CommaValues<T>() where T : struct {
            var names = Enum.GetNames(typeof(T));
            var ret = "";
            foreach (var item in names)
                ret += item + ",";
            return ret.TrimEnd(',');
        }
    }
}
