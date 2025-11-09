using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ElectronNET.API.Entities;

namespace ElectronNET.API
{
    /// <summary>
    /// Electron's process object is extended from the Node.js process object. It adds the
    /// events, properties, and methods.
    /// </summary>
    public sealed class Process
    {
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
        public Task<string> ExecPathAsync
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<string>();

                BridgeConnector.Socket.On("process-execPath-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-execPath-Completed");
                    taskCompletionSource.SetResult(result.ToString());
                });

                BridgeConnector.Socket.Emit("process-execPath");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<string[]>();

                BridgeConnector.Socket.On("process-argv-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-argv-Completed");
                    taskCompletionSource.SetResult(((JArray)result).ToObject<string[]>());
                });

                BridgeConnector.Socket.Emit("process-argv");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<string>();

                BridgeConnector.Socket.On("process-type-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-type-Completed");
                    taskCompletionSource.SetResult(result.ToString());
                });

                BridgeConnector.Socket.Emit("process-type");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<ProcessVersions>();

                BridgeConnector.Socket.On("process-versions-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-versions-Completed");
                    taskCompletionSource.SetResult(((JObject)result).ToObject<ProcessVersions>());
                });

                BridgeConnector.Socket.Emit("process-versions");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<bool>();

                BridgeConnector.Socket.On("process-defaultApp-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-defaultApp-Completed");
                    taskCompletionSource.SetResult(bool.Parse(result.ToString()));
                });

                BridgeConnector.Socket.Emit("process-defaultApp");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<bool>();

                BridgeConnector.Socket.On("process-isMainFrame-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-isMainFrame-Completed");
                    taskCompletionSource.SetResult(bool.Parse(result.ToString()));
                });

                BridgeConnector.Socket.Emit("process-isMainFrame");
                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// A String representing the path to the resources directory.
        /// </summary>
        public Task<string> ResourcesPathAsync
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<string>();

                BridgeConnector.Socket.On("process-resourcesPath-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-resourcesPath-Completed");
                    taskCompletionSource.SetResult(result.ToString());
                });

                BridgeConnector.Socket.Emit("process-resourcesPath");
                return taskCompletionSource.Task;
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
                var taskCompletionSource = new TaskCompletionSource<double>();

                BridgeConnector.Socket.On("process-uptime-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-uptime-Completed");
                    taskCompletionSource.SetResult(double.Parse(result.ToString()));
                });

                BridgeConnector.Socket.Emit("process-uptime");
                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// The PID of the electron process
        /// </summary>
        public Task<int> PidAsync
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<int>();

                BridgeConnector.Socket.On("process-pid-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-pid-Completed");
                    taskCompletionSource.SetResult(int.Parse(result.ToString()));
                });

                BridgeConnector.Socket.Emit("process-pid");
                return taskCompletionSource.Task;
            }
        }


        /// <summary>
        /// The operating system CPU architecture for which the Node.js binary was compiled
        /// </summary>
        public Task<string> ArchAsync
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<string>();

                BridgeConnector.Socket.On("process-arch-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-arch-Completed");
                    taskCompletionSource.SetResult(result.ToString());
                });

                BridgeConnector.Socket.Emit("process-arch");
                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// A string identifying the operating system platform on which the Node.js process is running
        /// </summary>
        public Task<string> PlatformAsync
        {
            get
            {
                var taskCompletionSource = new TaskCompletionSource<string>();

                BridgeConnector.Socket.On("process-platform-Completed", (result) =>
                {
                    BridgeConnector.Socket.Off("process-platform-Completed");
                    taskCompletionSource.SetResult(result.ToString());
                });

                BridgeConnector.Socket.Emit("process-platform");
                return taskCompletionSource.Task;
            }
        }
    }
}