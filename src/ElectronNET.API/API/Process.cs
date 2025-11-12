using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Electron's process object is extended from the Node.js process object. It adds the
    /// events, properties, and methods.
    /// </summary>
    public sealed class Process: ApiBase
    {
        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.DashesLowerFirst;
        internal Process()
        {
        }

        internal static Process Instance
        {
            get
            {
                if (_process == null)
                {
                    lock (_syncRoot)
                    {
                        if (_process == null)
                        {
                            _process = new Process();
                        }
                    }
                }

                return _process;
            }
        }

        private static Process _process;

        private static readonly object _syncRoot = new();

        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that
        /// started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary> 
        public Task<string> ExecPathAsync => GetPropertyAsync<string>();

        /// <summary>
        /// The process.argv property returns an array containing the command-line arguments passed
        /// when the Node.js process was launched. The first element will be process.execPath. See
        /// process.argv0 if access to the original value of argv[0] is needed. The second element
        /// will be the path to the JavaScript file being executed. The remaining elements will be
        /// any additional command-line arguments
        /// </summary>
        public Task<string[]> ArgvAsync => GetPropertyAsync<string[]>();

        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that
        /// started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary>
        public Task<string> TypeAsync => GetPropertyAsync<string>();

        /// <summary>
        /// The process.versions property returns an object listing the version strings of
        /// chrome and electron.
        /// </summary> 
        public Task<ProcessVersions> VersionsAsync => GetPropertyAsync<ProcessVersions>();

        /// <summary>
        /// A Boolean. When app is started by being passed as parameter to the default app, this
        /// property is true in the main process, otherwise it is false.
        /// </summary>
        public Task<bool> DefaultAppAsync => GetPropertyAsync<bool>();

        /// <summary>
        /// A Boolean, true when the current renderer context is the "main" renderer frame. If you
        /// want the ID of the current frame you should use webFrame.routingId
        /// </summary>
        public Task<bool> IsMainFrameAsync => GetPropertyAsync<bool>();

        /// <summary>
        /// A String representing the path to the resources directory.
        /// </summary>
        public Task<string> ResourcesPathAsync => GetPropertyAsync<string>();

        /// <summary>
        /// The number of seconds the current Node.js process has been running. The return value
        /// includes fractions of a second. Use Math.floor() to get whole seconds.
        /// </summary>
        public Task<double> UpTimeAsync => GetPropertyAsync<double>();

        /// <summary>
        /// The PID of the electron process
        /// </summary>
        public Task<int> PidAsync => GetPropertyAsync<int>();

        /// <summary>
        /// The operating system CPU architecture for which the Node.js binary was compiled
        /// </summary>
        public Task<string> ArchAsync => GetPropertyAsync<string>();

        /// <summary>
        /// A string identifying the operating system platform on which the Node.js process is running
        /// </summary>
        public Task<string> PlatformAsync => GetPropertyAsync<string>();
    }
}
