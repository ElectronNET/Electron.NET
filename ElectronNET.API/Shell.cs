using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Manage files and URLs using their default applications.
    /// </summary>
    public sealed class Shell
    {
        private static Shell _shell;
        private static object _syncRoot = new Object();

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
        /// <param name="fullPath"></param>
        /// <returns>Whether the item was successfully shown.</returns>
        public Task<bool> ShowItemInFolderAsync(string fullPath)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-showItemInFolderCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-showItemInFolderCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-showItemInFolder", fullPath);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given file in the desktop’s default manner.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>Whether the item was successfully opened.</returns>
        public Task<bool> OpenItemAsync(string fullPath)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-openItemCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-openItemCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-openItem", fullPath);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner. 
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Whether an application was available to open the URL. 
        /// If callback is specified, always returns true.</returns>
        public Task<bool> OpenExternalAsync(string url)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-openExternalCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-openExternalCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-openExternal", url);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner. 
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options">macOS only</param>
        /// <returns>Whether an application was available to open the URL. 
        /// If callback is specified, always returns true.</returns>
        public Task<bool> OpenExternalAsync(string url, OpenExternalOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-openExternalCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-openExternalCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-openExternal", url, JObject.FromObject(options, _jsonSerializer));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner. 
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options">macOS only</param>
        /// <param name="action">macOS only</param>
        /// <returns>Whether an application was available to open the URL. 
        /// If callback is specified, always returns true.</returns>
        public Task<bool> OpenExternalAsync(string url, OpenExternalOptions options, Action<Error> action)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-openExternalCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-openExternalCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Off("shell-openExternalCallback");
            BridgeConnector.Socket.On("shell-openExternalCallback", (args) => {
                var urlKey = ((JArray)args).First.ToString();
                var error = ((JArray)args).Last.ToObject<Error>();

                if(_openExternalCallbacks.ContainsKey(urlKey))
                {
                    _openExternalCallbacks[urlKey](error);
                }
            });

            _openExternalCallbacks.Add(url, action);

            BridgeConnector.Socket.Emit("shell-openExternal", url, JObject.FromObject(options, _jsonSerializer), true);

            return taskCompletionSource.Task;
        }

        private Dictionary<string, Action<Error>> _openExternalCallbacks = new Dictionary<string, Action<Error>>();

        /// <summary>
        /// Move the given file to trash and returns a boolean status for the operation.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        public Task<bool> MoveItemToTrashAsync(string fullPath)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-moveItemToTrashCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-moveItemToTrashCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-moveItemToTrash", fullPath);

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
        /// <param name="shortcutPath"></param>
        /// <param name="operation"></param>
        /// <param name="options"></param>
        /// <returns>Whether the shortcut was created successfully.</returns>
        public Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("shell-writeShortcutLinkCompleted", (success) =>
            {
                BridgeConnector.Socket.Off("shell-writeShortcutLinkCompleted");

                taskCompletionSource.SetResult((bool)success);
            });

            BridgeConnector.Socket.Emit("shell-writeShortcutLink", shortcutPath, operation.ToString(), JObject.FromObject(options, _jsonSerializer));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// 
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        public Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)
        {
            var taskCompletionSource = new TaskCompletionSource<ShortcutDetails>();

            BridgeConnector.Socket.On("shell-readShortcutLinkCompleted", (shortcutDetails) =>
            {
                BridgeConnector.Socket.Off("shell-readShortcutLinkCompleted");

                taskCompletionSource.SetResult((ShortcutDetails)shortcutDetails);
            });

            BridgeConnector.Socket.Emit("shell-readShortcutLink", shortcutPath);

            return taskCompletionSource.Task;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}