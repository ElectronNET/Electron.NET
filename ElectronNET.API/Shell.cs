using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using ElectronNET.API.Extensions;
using ElectronNET.API.Interfaces;
using System.Runtime.Versioning;

namespace ElectronNET.API
{
    /// <summary>
    /// Manage files and URLs using their default applications.
    /// </summary>
    public sealed class Shell : IShell
    {
        private static Shell _shell;
        private static readonly object _syncRoot = new();

        internal Shell() { }

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
            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On("shell-showItemInFolderCompleted", () =>
            {
                BridgeConnector.Off("shell-showItemInFolderCompleted");
            });

            BridgeConnector.Emit("shell-showItemInFolder", fullPath);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given file in the desktop's default manner.
        /// </summary>
        /// <param name="path">The path to the directory / file.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public Task<string> OpenPathAsync(string path)
        {
            var taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<string>("shell-openPathCompleted", (errorMessage) =>
            {
                BridgeConnector.Off("shell-openPathCompleted");

                taskCompletionSource.SetResult(errorMessage);
            });

            BridgeConnector.Emit("shell-openPath", path);

            return taskCompletionSource.Task;
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
            var taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<string>("shell-openExternalCompleted", (error) =>
            {
                BridgeConnector.Off("shell-openExternalCompleted");

                taskCompletionSource.SetResult(error);
            });

            if (options == null)
            {
                BridgeConnector.Emit("shell-openExternal", url);
            }
            else
            {
                BridgeConnector.Emit("shell-openExternal", url, options);
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Move the given file to trash and returns a <see cref="bool"/> status for the operation.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        public Task<bool> TrashItemAsync(string fullPath)
        {
            return BridgeConnector.OnResult<bool>("shell-trashItem", "shell-trashItem-completed", fullPath);
        }

        /// <summary>
        /// Play the beep sound.
        /// </summary>
        public void Beep()
        {
            BridgeConnector.Emit("shell-beep");
        }

        /// <summary>
        /// Creates or updates a shortcut link at shortcutPath.
        /// </summary>
        /// <param name="shortcutPath">The path to the shortcut.</param>
        /// <param name="operation">Default is <see cref="ShortcutLinkOperation.Create"/></param>
        /// <param name="options">Structure of a shortcut.</param>
        /// <returns>Whether the shortcut was created successfully.</returns>
        [SupportedOSPlatform("windows")]
        public Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)
        {
            return BridgeConnector.OnResult<bool>("shell-writeShortcutLink", "shell-writeShortcutLinkCompleted", shortcutPath, operation.GetDescription(), options);
        }

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath">The path tot the shortcut.</param>
        /// <returns><see cref="ShortcutDetails"/> of the shortcut.</returns>
        [SupportedOSPlatform("windows")]
        public Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)
        {
            return BridgeConnector.OnResult<ShortcutDetails>("shell-readShortcutLink", "shell-readShortcutLinkCompleted", shortcutPath);
        }
    }
}
