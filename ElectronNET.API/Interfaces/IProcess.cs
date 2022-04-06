using System.Threading.Tasks;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Electron's process object is extended from the Node.js process object. It adds the
    /// events, properties, and methods.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that
        /// started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary> 
        Task<string> ExecPathAsync { get; }

        /// <summary>
        /// The process.argv property returns an array containing the command-line arguments passed
        /// when the Node.js process was launched. The first element will be process.execPath. See
        /// process.argv0 if access to the original value of argv[0] is needed. The second element
        /// will be the path to the JavaScript file being executed. The remaining elements will be
        /// any additional command-line arguments
        /// </summary>
        Task<string[]> ArgvAsync { get; }

        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that
        /// started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary>
        Task<string> TypeAsync { get; }

        /// <summary>
        /// The process.versions property returns an object listing the version strings of
        /// chrome and electron.
        /// </summary> 
        Task<ProcessVersions> VersionsAsync { get; }

        /// <summary>
        /// A Boolean. When app is started by being passed as parameter to the default app, this
        /// property is true in the main process, otherwise it is false.
        /// </summary>
        Task<bool> DefaultAppAsync { get; }

        /// <summary>
        /// A Boolean, true when the current renderer context is the "main" renderer frame. If you
        /// want the ID of the current frame you should use webFrame.routingId
        /// </summary>
        Task<bool> IsMainFrameAsync { get; }

        /// <summary>
        /// A String representing the path to the resources directory.
        /// </summary>
        Task<string> ResourcesPathAsync { get; }

        /// <summary>
        /// The number of seconds the current Node.js process has been running. The return value
        /// includes fractions of a second. Use Math.floor() to get whole seconds.
        /// </summary>
        Task<double> UpTimeAsync { get; }

        /// <summary>
        /// The PID of the electron process
        /// </summary>
        Task<int> PidAsync { get; }

        /// <summary>
        /// The operating system CPU architecture for which the Node.js binary was compiled
        /// </summary>
        Task<string> ArchAsync { get; }

        /// <summary>
        /// A string identifying the operating system platform on which the Node.js process is running
        /// </summary>
        Task<string> PlatformAsync { get; }
    }
}