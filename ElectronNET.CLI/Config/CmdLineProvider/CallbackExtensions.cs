using Microsoft.Extensions.Configuration;
using System;

namespace ElectronNET.CLI.Config.CmdLineProvider {

    /// <summary> Extension methods for <see cref="IConfigurationBuilder"/>. </summary>
    public static class CallbackExtensions {

        /// <summary> Extension method for a direct callback. </summary>
        /// <param name="builder"> The configurationBuilder to act on. </param>
        /// <param name="args"> The command line options. </param>
        /// <param name="loadaction"> The function pointer for the callback. </param>
        /// <returns> An <see cref="IConfigurationBuilder"/>. </returns>
        public static IConfigurationBuilder AddCmdLineDirectCallback(this IConfigurationBuilder builder, string[] args, DirectCallbackProvider.DirectLoadDelegate loadaction) {
            builder.Add(new DirectCallbackSource { Args = args, DirectLoadFunc = loadaction });
            return builder;
        }

        /// <summary> Extension method for a direct callback. </summary>
        /// <param name="builder"> The builder to act on. </param>
        /// <param name="configureSource"> The configure source. </param>
        /// <returns> An <see cref="IConfigurationBuilder"/>. </returns>
        public static IConfigurationBuilder AddCmdLineDirectCallback(this IConfigurationBuilder builder, Action<DirectCallbackSource> configureSource)
            => builder.Add(configureSource);

    }
}
