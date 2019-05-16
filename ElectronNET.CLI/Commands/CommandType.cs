using ElectronNET.CLI.Config.Commands;

namespace ElectronNET.CLI.Commands {

    /// <summary> List of available commands. </summary>
    public enum CommandType {

        /// <summary> List the version of the application. </summary>
        version,

        /// <summary> initialize the project. </summary>
        init,

        /// <summary> Start debugging the project </summary>
        start,

        /// <summary> Build the project </summary>
        build,

        /// <summary> Add a custom npm packages to the Electron Application </summary>
        add,

    }

    /// <summary> Extension methods for CommandType. </summary>
    public static class CommandTypeExtensions {

        /// <summary> Determine the install command to use for the package manager. </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns> The install command to use. </returns>
        public static ICommand ToCmdObject(this CommandType? type) {
            switch (type) {
                case CommandType.version:
                    return new VersionCommand();
                case CommandType.init:
                    return new InitCommand();
                case CommandType.start:
                    return new StartCommand();
                case CommandType.build:
                    return new BuildCommand();
                case CommandType.add:
                    return new AddCommand();
                default:
                    return null;
            }
        }

        /// <summary> Get a new instance of a configuration class for the given command. </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns> A new instance of a config class for the command. </returns>
        public static ICommandConfig NewConfig(this CommandType? type) {
            switch (type) {
                case CommandType.version:
                    return null;
                case CommandType.init:
                    return new InitConfig();
                case CommandType.start:
                    return new StartConfig();
                case CommandType.build:
                    return new BuildConfig();
                case CommandType.add:
                    return new AddConfig();
                default:
                    return null;
            }
        }
    }
}
