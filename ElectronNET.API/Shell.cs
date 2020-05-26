using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
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
            var taskCompletionSource = new TaskCompletionSource<object>();

            BridgeConnector.Socket.On("shell-showItemInFolderCompleted", () =>
            {
                BridgeConnector.Socket.Off("shell-showItemInFolderCompleted");
            });

            BridgeConnector.Socket.Emit("shell-showItemInFolder", fullPath);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given file in the desktop's default manner.
        /// </summary>
        /// <param name="path">The path to the directory / file.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public Task<string> OpenPathAsync(string path)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("shell-openPathCompleted", (errorMessage) =>
            {
                BridgeConnector.Socket.Off("shell-openPathCompleted");

                taskCompletionSource.SetResult((string) errorMessage);
            });

            BridgeConnector.Socket.Emit("shell-openPath", path);

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
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("shell-openExternalCompleted", (error) =>
            {
                BridgeConnector.Socket.Off("shell-openExternalCompleted");

                taskCompletionSource.SetResult((string) error);
            });

            if (options == null)
            {
                BridgeConnector.Socket.Emit("shell-openExternal", url);
            }
            else
            {
                BridgeConnector.Socket.Emit("shell-openExternal", url, JObject.FromObject(options, _jsonSerializer));
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Move the given file to trash and returns a <see cref="bool"/> status for the operation.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        /// <param name="deleteOnFail">Whether or not to unilaterally remove the item if the Trash is disabled or unsupported on the volume.</param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        public Task<bool> MoveItemToTrashAsync(string fullPath, bool deleteOnFail)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-moveItemToTrashCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-moveItemToTrashCompleted");

                taskCompletionSource.SetResult((bool) success);
            });

            BridgeConnector.Socket.Emit("shell-moveItemToTrash", fullPath, deleteOnFail);

            return taskCompletionSource.Task;
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
        public Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-writeShortcutLinkCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-writeShortcutLinkCompleted");

                taskCompletionSource.SetResult((bool) success);
            });

            BridgeConnector.Socket.Emit("shell-writeShortcutLink", shortcutPath, operation.GetDescription(), JObject.FromObject(options, _jsonSerializer));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath">The path tot the shortcut.</param>
        /// <returns><see cref="ShortcutDetails"/> of the shortcut.</returns>
        public Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)
        {
            var taskCompletionSource = new TaskCompletionSource<ShortcutDetails>();

            BridgeConnector.Socket.On("shell-readShortcutLinkCompleted", (shortcutDetails) =>
            {
                BridgeConnector.Socket.Off("shell-readShortcutLinkCompleted");

                var shortcutObject = shortcutDetails as JObject;
                var details = shortcutObject?.ToObject<ShortcutDetails>();

                taskCompletionSource.SetResult(details);
            });

            BridgeConnector.Socket.Emit("shell-readShortcutLink", shortcutPath);

            return taskCompletionSource.Task;
        }

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
