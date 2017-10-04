namespace ElectronNET.CLI.Commands
{
    /// <summary>
    /// The definitionn of an option for a command.
    /// </summary>
    public class CommandOption
    {
        /// <summary>
        /// An enum for the possible values for an option
        /// </summary>
        public enum CommandOptionValueType { NoValue, StringValue, BoolValue, IntValue, CommaDelimitedList, KeyValuePairs }

        /// <summary>
        /// The name of the option.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The short form of the command line switch. This will start with just one dash e.g. -f for framework
        /// </summary>
        public string ShortSwitch { get; set; }

        /// <summary>
        /// The full form of the command line switch. This will start with two dashes e.g. --framework
        /// </summary>
        public string Switch { get; set; }

        /// <summary>
        /// The description of the option
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of value that is expected with this command option
        /// </summary>
        public CommandOptionValueType ValueType { get; set; }

        /// <summary>
        /// The JSON key used in configuration file.`
        /// </summary>
        public string ConfigFileKey
        {
            get
            {
                var key = this.Switch;
                if (key.StartsWith("--"))
                    key = key.Substring(2);

                return key;
            }
        }
    }
}