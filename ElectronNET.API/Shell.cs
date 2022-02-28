using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
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
        public async Task<bool> ShowItemInFolderAsync(string fullPath)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("shell-showItemInFolder", fullPath);
        }

        /// <summary>
        /// Open the given file in the desktop's default manner.
        /// </summary>
        /// <param name="path">The path to the directory / file.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        public async Task<string> OpenPathAsync(string path)
        {
            return await SignalrSerializeHelper.GetSignalrResultString("shell-openPath", path);
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
        public async Task<string> OpenExternalAsync(string url, OpenExternalOptions options)
        {
            if (options == null)
            {
                return await SignalrSerializeHelper.GetSignalrResultString("shell-openExternal", url);
            }
            else
            {
                return await SignalrSerializeHelper.GetSignalrResultString("shell-openExternal", url, JObject.FromObject(options, _jsonSerializer));
            }
        }

        /// <summary>
        /// Move the given file to trash and returns a <see cref="bool"/> status for the operation.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        public async Task<bool> TrashItemAsync(string fullPath)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("shell-trashItem", fullPath);
        }

        /// <summary>
        /// Play the beep sound.
        /// </summary>
        public async void Beep()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("shell-beep");
        }

        /// <summary>
        /// Creates or updates a shortcut link at shortcutPath.
        /// </summary>
        /// <param name="shortcutPath">The path to the shortcut.</param>
        /// <param name="operation">Default is <see cref="ShortcutLinkOperation.Create"/></param>
        /// <param name="options">Structure of a shortcut.</param>
        /// <returns>Whether the shortcut was created successfully.</returns>
        public async Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("shell-writeShortcutLink", shortcutPath, operation.GetDescription(), JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath">The path tot the shortcut.</param>
        /// <returns><see cref="ShortcutDetails"/> of the shortcut.</returns>
        public async Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("shell-readShortcutLink", shortcutPath);
            return signalrResult?.ToObject<ShortcutDetails>();
        }

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
