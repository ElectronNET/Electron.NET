using System.Threading;
using System.Threading.Tasks;
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
        /// The process.execPath property returns the absolute pathname of the executable that started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary>
        /// <example>
        /// <code>
        /// var path = await Electron.Process.ExecPathAsync;
        /// </code>
        /// </example>
        public Task<string> ExecPathAsync
        {
            get
            {
                CancellationToken cancellationToken = new();
                cancellationToken.ThrowIfCancellationRequested();

                var taskCompletionSource = new TaskCompletionSource<string>();
                using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
                {
                    BridgeConnector.Socket.On("process-execPathCompleted", (text) =>
                    {
                        BridgeConnector.Socket.Off("process-execPathCompleted");
                        taskCompletionSource.SetResult((string) text);
                    });

                    BridgeConnector.Socket.Emit("process-execPath");

                    return taskCompletionSource.Task;
                }
            }
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <value></value>
        public Task<string[]> ArgvAsync
        {
            get
            {
                CancellationToken cancellationToken = new();
                cancellationToken.ThrowIfCancellationRequested();

                var taskCompletionSource = new TaskCompletionSource<string[]>();
                using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
                {
                    BridgeConnector.Socket.On("process-argvCompleted", (value) =>
                    {
                        BridgeConnector.Socket.Off("process-argvCompleted");
                        taskCompletionSource.SetResult( ((JArray)value).ToObject<string[]>() );
                    });

                BridgeConnector.Socket.Emit("process-argv");

                    return taskCompletionSource.Task;
                }
            }
        }

        /// <summary>
        /// The process.execPath property returns the absolute pathname of the executable that started the Node.js process. Symbolic links, if any, are resolved.
        /// </summary>
        /// <example>
        /// <code>
        /// var path = await Electron.Process.ExecPathAsync;
        /// </code>
        /// </example>
        public Task<string> TypeAsync
        {
            get
            {
                CancellationToken cancellationToken = new();
                cancellationToken.ThrowIfCancellationRequested();

                var taskCompletionSource = new TaskCompletionSource<string>();
                using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
                {
                    BridgeConnector.Socket.On("process-typeCompleted", (text) =>
                    {
                        BridgeConnector.Socket.Off("process-typeCompleted");
                        taskCompletionSource.SetResult((string) text);
                    });

                    BridgeConnector.Socket.Emit("process-type");

                    return taskCompletionSource.Task;
                }
            }
        }
    }
}
