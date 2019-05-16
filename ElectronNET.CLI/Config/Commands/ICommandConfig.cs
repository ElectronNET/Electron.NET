using Microsoft.Extensions.Configuration;

namespace ElectronNET.CLI.Config.Commands {

    /// <summary> Interface for command configuration. </summary>
    public interface ICommandConfig {

        /// <summary> Binds the settings based on the builder. </summary>
        /// <param name="builder"> The configuration builder. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool Bind(IConfiguration builder);

        /// <summary> Print the Usage. </summary>
        void PrintUsage();

    }
}
