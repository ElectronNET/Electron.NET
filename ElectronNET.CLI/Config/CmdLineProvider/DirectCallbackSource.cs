using Microsoft.Extensions.Configuration;

namespace ElectronNET.CLI.Config.CmdLineProvider {

    /// <summary> Command line source for a direct callback. </summary>
    public class DirectCallbackSource : IConfigurationSource {

        /// <summary> The command line options. </summary>
        /// <value> Command line options. </value>
        public string[] Args { get; set; }

        /// <summary> Function pointer for the callback. </summary>
        public DirectCallbackProvider.DirectLoadDelegate DirectLoadFunc = null;

        /// <summary> Builds the <see cref="DirectCallbackProvider"/> for this source. </summary>
        /// <param name="builder"> The <see cref="IConfigurationBuilder"/>. </param>
        /// <returns> A <see cref="DirectCallbackProvider"/> </returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new DirectCallbackProvider(Args, DirectLoadFunc);
        }
    }
}
