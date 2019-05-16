using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ElectronNET.CLI.Config.CmdLineProvider {

    /// <summary> Command line provider with a direct callback. </summary>
    public class DirectCallbackProvider : ConfigurationProvider {

        /// <summary> The command line options. </summary>
        /// <value> Command line options. </value>
        protected string[] Args { get; }

        /// <summary> Delegate type for a direct callback when parsing command line options. </summary>
        /// <param name="opts"> The command line options. </param>
        /// <returns> A Dictionary of key value pairs to use </returns>
        public delegate Dictionary<string, string> DirectLoadDelegate(string[] opts);

        /// <summary> Function pointer for the callback. </summary>
        public DirectLoadDelegate DirectLoadFunc;

        /// <summary> Initializes a new instance. </summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <param name="opts"> The command line options. </param>
        /// <param name="loadfunc"> A delegate pointer to a function to call back to</param>
        public DirectCallbackProvider(string[] opts, DirectLoadDelegate loadfunc) {
            Args = opts ?? throw new ArgumentNullException(nameof(opts));
            DirectLoadFunc = loadfunc ?? throw new ArgumentNullException(nameof(loadfunc));
        }

        /// <summary> Callback to parse the command line options. </summary>
        public override void Load() {
            Data = DirectLoadFunc(Args);
        }
    }
}
