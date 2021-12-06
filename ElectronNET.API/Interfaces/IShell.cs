using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Manage files and URLs using their default applications.
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// Show the given file in a file manager. If possible, select the file.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        Task ShowItemInFolderAsync(string fullPath);

        /// <summary>
        /// Open the given file in the desktop's default manner.
        /// </summary>
        /// <param name="path">The path to the directory / file.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        Task<string> OpenPathAsync(string path);

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner. 
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url">Max 2081 characters on windows.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        Task<string> OpenExternalAsync(string url);

        /// <summary>
        /// Open the given external protocol URL in the desktop’s default manner. 
        /// (For example, mailto: URLs in the user’s default mail agent).
        /// </summary>
        /// <param name="url">Max 2081 characters on windows.</param>
        /// <param name="options">Controls the behavior of OpenExternal.</param>
        /// <returns>The error message corresponding to the failure if a failure occurred, otherwise <see cref="string.Empty"/>.</returns>
        Task<string> OpenExternalAsync(string url, OpenExternalOptions options);

        /// <summary>
        /// Move the given file to trash and returns a <see cref="bool"/> status for the operation.
        /// </summary>
        /// <param name="fullPath">The full path to the directory / file.</param>
        /// <returns> Whether the item was successfully moved to the trash.</returns>
        Task<bool> TrashItemAsync(string fullPath);

        /// <summary>
        /// Play the beep sound.
        /// </summary>
        void Beep();

        /// <summary>
        /// Creates or updates a shortcut link at shortcutPath.
        /// </summary>
        /// <param name="shortcutPath">The path to the shortcut.</param>
        /// <param name="operation">Default is <see cref="ShortcutLinkOperation.Create"/></param>
        /// <param name="options">Structure of a shortcut.</param>
        /// <returns>Whether the shortcut was created successfully.</returns>
        Task<bool> WriteShortcutLinkAsync(string shortcutPath, ShortcutLinkOperation operation, ShortcutDetails options);

        /// <summary>
        /// Resolves the shortcut link at shortcutPath.
        /// An exception will be thrown when any error happens.
        /// </summary>
        /// <param name="shortcutPath">The path tot the shortcut.</param>
        /// <returns><see cref="ShortcutDetails"/> of the shortcut.</returns>
        Task<ShortcutDetails> ReadShortcutLinkAsync(string shortcutPath);
    }
}