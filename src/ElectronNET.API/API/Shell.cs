using System.Runtime.Versioning;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;

namespace ElectronNET.API
{
    /// <summary>
    /// Manage files and URLs using their default applications.
    /// </summary>
    public sealed class Shell
    {
        private static Shell _shell;
        private static object _syncRoot = new object();

        internal Shell()
        {
        }

        internal static Shell Instance
        {
            get
            {
                if (_shell == null)
                {
                    lock (_syncRoot)
                    {
                        if (_shell == null)
                        {
                            _shell = new Shell();
                        }
                    }
                }

                return _shell;
            }
        }

        /// <summary>
        /// Show the given file in a file manager. If possible, select the file.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        public Task ShowItemInFolderAsync(string fullPath)
        {
            BridgeConnector.Socket.Emit("shell-showItemInFolder", fullPath);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Open the given file in the desktop's default manner.
        /// </summary>
        /// <param name="path">The path to the directory / file.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public Task<string> OpenPathAsync(string path)
        {
            var tcs = new TaskCompletionSource<string>();

            BridgeConnector.Socket.Once<string>("shell-openPathCompleted", tcs.SetResult);
            BridgeConnector.Socket.Emit("shell-openPath", path);

            return tcs.Task;
        }

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner.
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url">Max 2081 characters on windows.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public Task<string> OpenExternalAsync(string url)
        {
            return OpenExternalAsync(url, null);
        }

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner.
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url">Max 2081 characters on windows.</param>
        /// <param name="options">Controls the behavior of OpenExternal.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public Task<string> OpenExternalAsync(string url, OpenExternalOptions options)
        {
            var tcs = new TaskCompletionSource<string>();

            BridgeConnector.Socket.Once<string>("shell-openExternalCompleted", tcs.SetResult);

            if (options == null)
            {
                BridgeConnector.Socket.Emit("shell-openExternal", url);
            }
            else
            {
                BridgeConnector.Socket.Emit("shell-openExternal", url, options);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Move the given file to trash and returns a <see cref="bool"/> status for the operation.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        public Task<bool> TrashItemAsync(string fullPath)
        {
            var tcs = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.Once<bool>("shell-trashItem-completed", tcs.SetResult);
            BridgeConnector.Socket.Emit("shell-trashItem", fullPath);

            return tcs.Task;
        }

        /// <summary>
        /// Play the beep sound.
        /// </summary>
        public void Beep()
        {
            BridgeConnector.Socket.Emit("shell-beep");
        }

        /// <summary>
        /// Creates or updates a shortcut link at shortcutPath.
        /// </summary>
        /// <param name="shortcutPath">The path to the shortcut.</param>
        /// <param name="operation">Default is <see cref="ShortcutLinkOperation.Create"/></param>
        /// <param name="options">Structure of a shortcut.</param>
        /// <returns>Whether the shortcut was created successfully.</returns>
        [SupportedOSPlatform("Windows")]
        public Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)
        {
            var tcs = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.Once<bool>("shell-writeShortcutLinkCompleted", tcs.SetResult);
            BridgeConnector.Socket.Emit("shell-writeShortcutLink", shortcutPath, operation, options);

            return tcs.Task;
        }

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath">The path tot the shortcut.</param>
        /// <returns><see cref="ShortcutDetails"/> of the shortcut.</returns>
        [SupportedOSPlatform("Windows")]
        public Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)
        {
            var tcs = new TaskCompletionSource<ShortcutDetails>();

            BridgeConnector.Socket.Once<ShortcutDetails>("shell-readShortcutLinkCompleted", tcs.SetResult);
            BridgeConnector.Socket.Emit("shell-readShortcutLink", shortcutPath);

            return tcs.Task;
        }
    }
}