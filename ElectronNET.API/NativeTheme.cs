using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Read and respond to changes in Chromium's native color theme.
    /// </summary>
    public sealed class NativeTheme
    {
        private static NativeTheme _nativeTheme;
        private static object _syncRoot = new object();

        internal NativeTheme() { }

        internal static NativeTheme Instance
        {
            get
            {
                if (_nativeTheme == null)
                {
                    lock (_syncRoot)
                    {
                        if (_nativeTheme == null)
                        {
                            _nativeTheme = new NativeTheme();
                        }
                    }
                }

                return _nativeTheme;
            }
        }

        /// <summary>
        /// A `Boolean` for if the OS / Chromium currently has a dark mode enabled or is
        /// being instructed to show a dark-style UI.If you want to modify this value you
        /// should use `themeSource` below.
        /// </summary>
        /// <returns></returns>
        public Task<bool> ShouldUseDarkColorsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("nativeTheme-shouldUseDarkColors-completed", (shouldUseDarkColors) => {
                BridgeConnector.Socket.Off("nativeTheme-shouldUseDarkColors-completed");

                taskCompletionSource.SetResult((bool)shouldUseDarkColors);
            });

            BridgeConnector.Socket.Emit("nativeTheme-shouldUseDarkColors");

            return taskCompletionSource.Task;
        }
    }
}
