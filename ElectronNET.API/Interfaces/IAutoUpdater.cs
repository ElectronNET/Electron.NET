using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public interface IAutoUpdater
    {
        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        bool AutoDownload { set; }

        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        Task<bool> IsAutoDownloadEnabledAsync { get; }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        bool AutoInstallOnAppQuit { set; }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        Task<bool> IsAutoInstallOnAppQuitEnabledAsync { get; }

        /// <summary>
        /// *GitHub provider only.* Whether to allow update to pre-release versions. 
        /// Defaults to "true" if application version contains prerelease components (e.g. "0.12.1-alpha.1", here "alpha" is a prerelease component), otherwise "false".
        /// 
        /// If "true", downgrade will be allowed("allowDowngrade" will be set to "true").
        /// </summary>
        bool AllowPrerelease { set; }

        /// <summary>
        /// *GitHub provider only.* Whether to allow update to pre-release versions. 
        /// Defaults to "true" if application version contains prerelease components (e.g. "0.12.1-alpha.1", here "alpha" is a prerelease component), otherwise "false".
        /// 
        /// If "true", downgrade will be allowed("allowDowngrade" will be set to "true").
        /// </summary>
        Task<bool> IsAllowPrereleaseEnabledAsync { get; }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        bool FullChangelog { set; }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        Task<bool> IsFullChangeLogEnabledAsync { get; }

        /// <summary>
        /// Whether to allow version downgrade (when a user from the beta channel wants to go back to the stable channel).
        /// Taken in account only if channel differs (pre-release version component in terms of semantic versioning).
        /// Default is false.
        /// </summary>
        bool AllowDowngrade { set; }

        Task<bool> IsAllowDowngradeEnabledAsync { get; }

        /// <summary>
        /// For test only.
        /// </summary>
        Task<string> GetUpdateConfigPathAsync { get; }

        /// <summary>
        /// The current application version
        /// </summary>
        Task<SemVer> GetCurrentVersionAsync { get; }

        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        Task<string> GetChannelAsync { get; }

        /// <summary>
        /// The request headers.
        /// </summary>
        Task<Dictionary<string, string>> GetRequestHeadersAsync { get; }

        /// <summary>
        /// The request headers.
        /// </summary>
        Dictionary<string, string> RequestHeaders { set; }

        /// <summary>
        /// Emitted when there is an error while updating.
        /// </summary>
        event Action<string> OnError;

        /// <summary>
        /// Emitted when checking if an update has started.
        /// </summary>
        event Action OnCheckingForUpdate;

        /// <summary>
        /// Emitted when there is an available update. 
        /// The update is downloaded automatically if AutoDownload is true.
        /// </summary>
        event Action<UpdateInfo> OnUpdateAvailable;

        /// <summary>
        /// Emitted when there is no available update.
        /// </summary>
        event Action<UpdateInfo> OnUpdateNotAvailable;

        /// <summary>
        /// Emitted on download progress.
        /// </summary>
        event Action<ProgressInfo> OnDownloadProgress;

        /// <summary>
        /// Emitted on download complete.
        /// </summary>
        event Action<UpdateInfo> OnUpdateDownloaded;

        /// <summary>
        /// Asks the server whether there is an update.
        /// </summary>
        /// <returns></returns>
        Task<UpdateCheckResult> CheckForUpdatesAsync();

        /// <summary>
        /// Asks the server whether there is an update.
        /// 
        /// This will immediately download an update, then install when the app quits.
        /// </summary>
        /// <returns></returns>
        Task<UpdateCheckResult> CheckForUpdatesAndNotifyAsync();

        /// <summary>
        ///  Restarts the app and installs the update after it has been downloaded.
        ///  It should only be called after `update-downloaded` has been emitted.
        ///  
        ///  Note: QuitAndInstall() will close all application windows first and only emit `before-quit` event on `app` after that.
        ///  This is different from the normal quit event sequence.
        /// </summary>
        /// <param name="isSilent">*windows-only* Runs the installer in silent mode. Defaults to `false`.</param>
        /// <param name="isForceRunAfter">Run the app after finish even on silent install. Not applicable for macOS. Ignored if `isSilent` is set to `false`.</param>
        void QuitAndInstall(bool isSilent = false, bool isForceRunAfter = false);

        /// <summary>
        /// Start downloading update manually. You can use this method if "AutoDownload" option is set to "false".
        /// </summary>
        /// <returns>Path to downloaded file.</returns>
        Task<string> DownloadUpdateAsync();

        /// <summary>
        /// Feed URL.
        /// </summary>
        /// <returns>Feed URL.</returns>
        Task<string> GetFeedURLAsync();
    }
}