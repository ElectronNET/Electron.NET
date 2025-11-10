using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using ElectronNET.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public sealed class AutoUpdater
    {
        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        public bool AutoDownload
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.Once<bool>("autoUpdater-autoDownload-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-autoDownload-get");

                    return tcs.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-autoDownload-set", value);
            }
        }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        public bool AutoInstallOnAppQuit
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.Once<bool>("autoUpdater-autoInstallOnAppQuit-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-autoInstallOnAppQuit-get");

                    return tcs.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-autoInstallOnAppQuit-set", value);
            }
        }

        /// <summary>
        /// *GitHub provider only.* Whether to allow update to pre-release versions. 
        /// Defaults to "true" if application version contains prerelease components (e.g. "0.12.1-alpha.1", here "alpha" is a prerelease component), otherwise "false".
        /// 
        /// If "true", downgrade will be allowed("allowDowngrade" will be set to "true").
        /// </summary>
        public bool AllowPrerelease
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.Once<bool>("autoUpdater-allowPrerelease-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-allowPrerelease-get");

                    return tcs.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-allowPrerelease-set", value);
            }
        }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        public bool FullChangelog
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.Once<bool>("autoUpdater-fullChangelog-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-fullChangelog-get");

                    return tcs.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-fullChangelog-set", value);
            }
        }

        /// <summary>
        /// Whether to allow version downgrade (when a user from the beta channel wants to go back to the stable channel).
        /// Taken in account only if channel differs (pre-release version component in terms of semantic versioning).
        /// Default is false.
        /// </summary>
        public bool AllowDowngrade
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.Once<bool>("autoUpdater-allowDowngrade-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-allowDowngrade-get");

                    return tcs.Task;
                }).Result;
            }
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-allowDowngrade-set", value);
            }
        }

        /// <summary>
        /// For test only.
        /// </summary>
        public string UpdateConfigPath
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.Once<string>("autoUpdater-updateConfigPath-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-updateConfigPath-get");

                    return tcs.Task;
                }).Result;
            }
        }

        /// <summary>
        /// The current application version
        /// </summary>
        public Task<SemVer> CurrentVersionAsync
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<SemVer>();

                    BridgeConnector.Socket.On<SemVer>("autoUpdater-currentVersion-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-currentVersion-get");

                    return tcs.Task;
                });
            }
        }

        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        [Obsolete("Use the asynchronous version ChannelAsync instead")]
        public string Channel
        {
            get
            {
                return ChannelAsync.Result;
            }
        }

        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        public Task<string> ChannelAsync
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On<string>("autoUpdater-channel-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-channel-get");

                    return tcs.Task;
                });
            }
        }


        /// <summary>
        /// The request headers.
        /// </summary>
        public Task<Dictionary<string, string>> RequestHeadersAsync
        {
            get
            {
                return Task.Run(() =>
                {
                    var tcs = new TaskCompletionSource<Dictionary<string, string>>();

                    BridgeConnector.Socket.On<Dictionary<string, string>>("autoUpdater-requestHeaders-get-reply", tcs.SetResult);
                    BridgeConnector.Socket.Emit("autoUpdater-requestHeaders-get");
                    
                    return tcs.Task;
                });
            }
        }

        /// <summary>
        /// The request headers.
        /// </summary>
        public Dictionary<string, string> RequestHeaders
        {
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-requestHeaders-set", value);
            }
        }

        /// <summary>
        /// Emitted when there is an error while updating.
        /// </summary>
        public event Action<string> OnError
        {
            add => ApiEventManager.AddEvent("autoUpdater-error", GetHashCode(), _error, value, (args) => args.ToString());
            remove => ApiEventManager.RemoveEvent("autoUpdater-error", GetHashCode(), _error, value);
        }

        private event Action<string> _error;

        /// <summary>
        /// Emitted when checking if an update has started.
        /// </summary>
        public event Action OnCheckingForUpdate
        {
            add => ApiEventManager.AddEvent("autoUpdater-checking-for-update", GetHashCode(), _checkingForUpdate, value);
            remove => ApiEventManager.RemoveEvent("autoUpdater-checking-for-update", GetHashCode(), _checkingForUpdate, value);
        }

        private event Action _checkingForUpdate;

        /// <summary>
        /// Emitted when there is an available update. 
        /// The update is downloaded automatically if AutoDownload is true.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateAvailable
        {
            add => ApiEventManager.AddEvent("autoUpdater-update-available", GetHashCode(), _updateAvailable, value, (args) => args.Deserialize(ElectronJsonContext.Default.UpdateInfo));
            remove => ApiEventManager.RemoveEvent("autoUpdater-update-available", GetHashCode(), _updateAvailable, value);
        }

        private event Action<UpdateInfo> _updateAvailable;

        /// <summary>
        /// Emitted when there is no available update.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateNotAvailable
        {
            add => ApiEventManager.AddEvent("autoUpdater-update-not-available", GetHashCode(), _updateNotAvailable, value, (args) => args.Deserialize(ElectronJsonContext.Default.UpdateInfo));
            remove => ApiEventManager.RemoveEvent("autoUpdater-update-not-available", GetHashCode(), _updateNotAvailable, value);
        }

        private event Action<UpdateInfo> _updateNotAvailable;

        /// <summary>
        /// Emitted on download progress.
        /// </summary>
        public event Action<ProgressInfo> OnDownloadProgress
        {
            add => ApiEventManager.AddEvent("autoUpdater-download-progress", GetHashCode(), _downloadProgress, value, (args) => args.Deserialize(ElectronJsonContext.Default.ProgressInfo));
            remove => ApiEventManager.RemoveEvent("autoUpdater-download-progress", GetHashCode(), _downloadProgress, value);
        }

        private event Action<ProgressInfo> _downloadProgress;

        /// <summary>
        /// Emitted on download complete.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateDownloaded
        {
            add => ApiEventManager.AddEvent("autoUpdater-update-downloaded", GetHashCode(), _updateDownloaded, value, (args) => args.Deserialize(ElectronJsonContext.Default.UpdateInfo));
            remove => ApiEventManager.RemoveEvent("autoUpdater-update-downloaded", GetHashCode(), _updateDownloaded, value);
        }

        private event Action<UpdateInfo> _updateDownloaded;

        private static AutoUpdater _autoUpdater;
        private static object _syncRoot = new object();

        internal AutoUpdater()
        {
        }

        internal static AutoUpdater Instance
        {
            get
            {
                if (_autoUpdater == null)
                {
                    lock (_syncRoot)
                    {
                        if (_autoUpdater == null)
                        {
                            _autoUpdater = new AutoUpdater();
                        }
                    }
                }

                return _autoUpdater;
            }
        }

        /// <summary>
        /// Asks the server whether there is an update.
        /// </summary>
        /// <returns></returns>
        public Task<UpdateCheckResult> CheckForUpdatesAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<UpdateCheckResult>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<UpdateCheckResult>("autoUpdaterCheckForUpdatesComplete" + guid, (result) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesError" + guid);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.Once<string>("autoUpdaterCheckForUpdatesError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                string message = "An error occurred in CheckForUpdatesAsync";
                if (!string.IsNullOrEmpty(result)) message = result;
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Socket.Emit("autoUpdaterCheckForUpdates", guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Asks the server whether there is an update.
        /// 
        /// This will immediately download an update, then install when the app quits.
        /// </summary>
        /// <returns></returns>
        public Task<UpdateCheckResult> CheckForUpdatesAndNotifyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<UpdateCheckResult>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<UpdateCheckResult>("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid, (result) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyError" + guid);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.Once<string>("autoUpdaterCheckForUpdatesAndNotifyError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                string message = "An error occurred in autoUpdaterCheckForUpdatesAndNotify";
                if (!string.IsNullOrEmpty(result)) message = result;
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Socket.Emit("autoUpdaterCheckForUpdatesAndNotify", guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        ///  Restarts the app and installs the update after it has been downloaded.
        ///  It should only be called after `update-downloaded` has been emitted.
        ///  
        ///  Note: QuitAndInstall() will close all application windows first and only emit `before-quit` event on `app` after that.
        ///  This is different from the normal quit event sequence.
        /// </summary>
        /// <param name="isSilent">*windows-only* Runs the installer in silent mode. Defaults to `false`.</param>
        /// <param name="isForceRunAfter">Run the app after finish even on silent install. Not applicable for macOS. Ignored if `isSilent` is set to `false`.</param>
        public void QuitAndInstall(bool isSilent = false, bool isForceRunAfter = false)
        {
            BridgeConnector.Socket.Emit("autoUpdaterQuitAndInstall", isSilent, isForceRunAfter);
        }

        /// <summary>
        /// Start downloading update manually. You can use this method if "AutoDownload" option is set to "false".
        /// </summary>
        /// <returns>Path to downloaded file.</returns>
        public Task<string> DownloadUpdateAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string>("autoUpdaterDownloadUpdateComplete" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("autoUpdaterDownloadUpdate", guid);

            return tcs.Task;
        }

        /// <summary>
        /// Feed URL.
        /// </summary>
        /// <returns>Feed URL.</returns>
        public Task<string> GetFeedURLAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On<string>("autoUpdaterGetFeedURLComplete" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("autoUpdaterGetFeedURL", guid);

            return tcs.Task;
        }


    }
}


