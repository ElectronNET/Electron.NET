using System.Threading;
using System.Threading.Tasks;

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
        /// TBD
        /// </summary>
        /// <value></value>
        public async Task<string> GetExecPathAsync(CancellationToken cancellationToken = default)
        {
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

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }
    }
}
