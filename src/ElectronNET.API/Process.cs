using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElectronNET.API
{
    /// <summary>
    /// Electron's process object is extended from the Node.js process object. It adds the
    /// events, properties, and methods.
    /// </summary>
    public sealed class Process
    {
        internal Process() { }

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
        public Task<string> ExecPathAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<string>(
                    "process-execPath", "process-execPath-Completed");
            }
        }

        /// <summary>
        /// The process.argv property returns an array containing the command-line arguments passed
        /// when the Node.js process was launched. The first element will be process.execPath. See
        /// process.argv0 if access to the original value of argv[0] is needed. The second element
        /// will be the path to the JavaScript file being executed. The remaining elements will be
        /// any additional command-line arguments
        /// </summary>
        public Task<string[]> ArgvAsync
        {
            get
            {
                return BridgeConnector.GetArrayOverSocketAsync<string[]>(
                    "process-argv", "process-argv-Completed");
            }
        }

        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that
        /// started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary>
        public Task<string> TypeAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<string>(
                    "process-type", "process-type-Completed");
            }
        }
    
    
        /// <summary>
        /// The process.versions property returns an object listing the version strings of
        /// chrome and electron.
        /// </summary> 
        public Task<ProcessVersions> VersionsAsync 
        {
            get
            {
                return BridgeConnector.GetObjectOverSocketAsync<ProcessVersions>(
                    "process-versions", "process-versions-Completed");
            }
        }
    
    
        /// <summary>
        /// A Boolean. When app is started by being passed as parameter to the default app, this
        /// property is true in the main process, otherwise it is false.
        /// </summary>
        public Task<bool> DefaultAppAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<bool>(
                    "process-defaultApp", "process-defaultApp-Completed");
            }
        }

        /// <summary>
        /// A Boolean, true when the current renderer context is the "main" renderer frame. If you
        /// want the ID of the current frame you should use webFrame.routingId
        /// </summary>
        public Task<bool> IsMainFrameAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<bool>(
                    "process-isMainFrame", "process-isMainFrame-Completed");
            }
        }

        /// <summary>
        /// A String representing the path to the resources directory.
        /// </summary>
        public Task<string> ResourcesPathAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<string>(
                    "process-resourcesPath", "process-resourcesPath-Completed");
            }
        }

        /// <summary>
        /// The number of seconds the current Node.js process has been running. The return value
        /// includes fractions of a second. Use Math.floor() to get whole seconds.
        /// </summary>
        public Task<double> UpTimeAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<double>(
                    "process-uptime", "process-uptime-Completed");
            }
        }

        /// <summary>
        /// The PID of the electron process
        /// </summary>
        public Task<int> PidAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<int>(
                    "process-pid", "process-pid-Completed");
            }
        }


        /// <summary>
        /// The operating system CPU architecture for which the Node.js binary was compiled
        /// </summary>
        public Task<string> ArchAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<string>(
                    "process-arch", "process-arch-Completed");
            }
        }

        /// <summary>
        /// A string identifying the operating system platform on which the Node.js process is running
        /// </summary>
        public Task<string> PlatformAsync
        {
            get
            {
                return BridgeConnector.GetValueOverSocketAsync<string>(
                    "process-platform", "process-platform-Completed");
            }
        }
    
    }
}
