using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Manipulate the command line arguments for your app that Chromium reads.
    /// </summary>
    public sealed class CommandLine
    {
        internal CommandLine() { }

        internal static CommandLine Instance
        {
            get
            {
                if (_commandLine == null)
                {
                    lock (_syncRoot)
                    {
                        if (_commandLine == null)
                        {
                            _commandLine = new CommandLine();
                        }
                    }
                }

                return _commandLine;
            }
        }

        private static CommandLine _commandLine;

        private static object _syncRoot = new object();

        /// <summary>
        /// Append a switch (with optional value) to Chromium's command line.
        /// </summary>
        /// <param name="the_switch">A command-line switch, without the leading --</param>
        /// <param name="value">(optional) - A value for the given switch</param>
        /// <remarks>
        /// Note: This will not affect process.argv. The intended usage of this function is to control Chromium's behavior.
        /// </remarks>
        public void AppendSwitch(string the_switch, string value = "")
        {
            BridgeConnector.Socket.Emit("appCommandLineAppendSwitch", the_switch, value);
        }

        /// <summary>
        /// Append an argument to Chromium's command line. The argument will be quoted correctly. Switches will precede arguments regardless of appending order.
        ///
        /// If you're appending an argument like <code>--switch=value</code>, consider using <code>appendSwitch('switch', 'value')</code> instead.
        /// </summary>
        /// <param name="value">The argument to append to the command line</param>
        /// <remarks>
        /// Note: This will not affect process.argv. The intended usage of this function is to control Chromium's behavior.
        /// </remarks>
        public void AppendArgument(string value)
        {
            BridgeConnector.Socket.Emit("appCommandLineAppendArgument", value);
        }

        /// <summary>
        /// Whether the command-line switch is present.
        /// </summary>
        /// <param name="switchName">A command-line switch</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether the command-line switch is present.</returns>
        public async Task<bool> HasSwitchAsync(string switchName, CancellationToken cancellationToken = default(CancellationToken)) 
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appCommandLineHasSwitchCompleted", (result) =>
                {
                    BridgeConnector.Socket.Off("appCommandLineHasSwitchCompleted");
                    taskCompletionSource.SetResult((bool)result);
                });

                BridgeConnector.Socket.Emit("appCommandLineHasSwitch", switchName);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The command-line switch value.
        /// </summary>
        /// <param name="switchName">A command-line switch</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The command-line switch value.</returns>
        /// <remarks>
        /// Note: When the switch is not present or has no value, it returns empty string.
        /// </remarks>
        public async Task<string> GetSwitchValueAsync(string switchName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On("appCommandLineGetSwitchValueCompleted", (result) =>
                {
                    BridgeConnector.Socket.Off("appCommandLineGetSwitchValueCompleted");
                    taskCompletionSource.SetResult((string)result);
                });

                BridgeConnector.Socket.Emit("appCommandLineGetSwitchValue", switchName);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }
    }
}
