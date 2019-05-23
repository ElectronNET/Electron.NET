using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

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
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("autoUpdater-autoDownload-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-autoDownload-get-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-autoDownload-get");

                    return taskCompletionSource.Task;
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
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("autoUpdater-autoInstallOnAppQuit-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-autoInstallOnAppQuit-get-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-autoInstallOnAppQuit-get");

                    return taskCompletionSource.Task;
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
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("autoUpdater-allowPrerelease-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-allowPrerelease-get-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-allowPrerelease-get");

                    return taskCompletionSource.Task;
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
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("autoUpdater-fullChangelog-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-fullChangelog-get-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-fullChangelog-get");

                    return taskCompletionSource.Task;
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
                return Task.Run<bool>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On("autoUpdater-allowDowngrade-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-allowDowngrade-get-reply");
                        taskCompletionSource.SetResult((bool)result);
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-allowDowngrade-get");

                    return taskCompletionSource.Task;
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
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On("autoUpdater-updateConfigPath-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-updateConfigPath-get-reply");
                        taskCompletionSource.SetResult(result.ToString());
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-updateConfigPath-get");

                    return taskCompletionSource.Task;
                }).Result;
            }
        }

        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        public string Channel
        {
            get
            {
                return Task.Run<string>(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On("autoUpdater-channel-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-channel-get-reply");
                        taskCompletionSource.SetResult(result.ToString());
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-channel-get");

                    return taskCompletionSource.Task;
                }).Result;
            }
        }

        /// <summary>
        /// Emitted when there is an error while updating.
        /// </summary>
        public event Action<string> OnError
        {
            add
            {
                if (_error == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-error" + GetHashCode(), (message) =>
                    {
                        _error(message.ToString());
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-error-event", GetHashCode());
                }
                _error += value;
            }
            remove
            {
                _error -= value;

                if (_error == null)
                    BridgeConnector.Socket.Off("autoUpdater-error" + GetHashCode());
            }
        }

        private event Action<string> _error;

        /// <summary>
        /// Emitted when checking if an update has started.
        /// </summary>
        public event Action OnCheckingForUpdate
        {
            add
            {
                if (_checkingForUpdate == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-checking-for-update" + GetHashCode(), () =>
                    {
                        _checkingForUpdate();
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-checking-for-update-event", GetHashCode());
                }
                _checkingForUpdate += value;
            }
            remove
            {
                _checkingForUpdate -= value;

                if (_checkingForUpdate == null)
                    BridgeConnector.Socket.Off("autoUpdater-checking-for-update" + GetHashCode());
            }
        }

        private event Action _checkingForUpdate;

        /// <summary>
        /// Emitted when there is an available update. 
        /// The update is downloaded automatically if AutoDownload is true.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateAvailable
        {
            add
            {
                if (_updateAvailable == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-update-available" + GetHashCode(), (updateInfo) =>
                    {
                        _updateAvailable(JObject.Parse(updateInfo.ToString()).ToObject<UpdateInfo>());
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-update-available-event", GetHashCode());
                }
                _updateAvailable += value;
            }
            remove
            {
                _updateAvailable -= value;

                if (_updateAvailable == null)
                    BridgeConnector.Socket.Off("autoUpdater-update-available" + GetHashCode());
            }
        }

        private event Action<UpdateInfo> _updateAvailable;

        /// <summary>
        /// Emitted when there is no available update.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateNotAvailable
        {
            add
            {
                if (_updateNotAvailable == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-update-not-available" + GetHashCode(), (updateInfo) =>
                    {
                        _updateNotAvailable(JObject.Parse(updateInfo.ToString()).ToObject<UpdateInfo>());
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-update-not-available-event", GetHashCode());
                }
                _updateNotAvailable += value;
            }
            remove
            {
                _updateNotAvailable -= value;

                if (_updateNotAvailable == null)
                    BridgeConnector.Socket.Off("autoUpdater-update-not-available" + GetHashCode());
            }
        }

        private event Action<UpdateInfo> _updateNotAvailable;

        /// <summary>
        /// Emitted on download progress.
        /// </summary>
        public event Action<ProgressInfo> OnDownloadProgress
        {
            add
            {
                if (_downloadProgress == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-download-progress" + GetHashCode(), (progressInfo) =>
                    {
                        _downloadProgress(JObject.Parse(progressInfo.ToString()).ToObject<ProgressInfo>());
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-download-progress-event", GetHashCode());
                }
                _downloadProgress += value;
            }
            remove
            {
                _downloadProgress -= value;

                if (_downloadProgress == null)
                    BridgeConnector.Socket.Off("autoUpdater-download-progress" + GetHashCode());
            }
        }

        private event Action<ProgressInfo> _downloadProgress;

        /// <summary>
        /// Emitted on download complete.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateDownloaded
        {
            add
            {
                if (_updateDownloaded == null)
                {
                    BridgeConnector.Socket.On("autoUpdater-update-downloaded" + GetHashCode(), (updateInfo) =>
                    {
                        _updateDownloaded(JObject.Parse(updateInfo.ToString()).ToObject<UpdateInfo>());
                    });

                    BridgeConnector.Socket.Emit("register-autoUpdater-update-downloaded-event", GetHashCode());
                }
                _updateDownloaded += value;
            }
            remove
            {
                _updateDownloaded -= value;

                if (_updateDownloaded == null)
                    BridgeConnector.Socket.Off("autoUpdater-update-downloaded" + GetHashCode());
            }
        }

        private event Action<UpdateInfo> _updateDownloaded;

        private static AutoUpdater _autoUpdater;
        private static object _syncRoot = new object();

        internal AutoUpdater() { }

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

            BridgeConnector.Socket.On("autoUpdaterCheckForUpdatesComplete" + guid, (updateCheckResult) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                taskCompletionSource.SetResult(JObject.Parse(updateCheckResult.ToString()).ToObject<UpdateCheckResult>());
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

            BridgeConnector.Socket.On("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid, (updateCheckResult) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                taskCompletionSource.SetResult(JObject.Parse(updateCheckResult.ToString()).ToObject<UpdateCheckResult>());
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
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("autoUpdaterDownloadUpdateComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterDownloadUpdateComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.ToString());
            });

            BridgeConnector.Socket.Emit("autoUpdaterDownloadUpdate", guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Feed URL.
        /// </summary>
        /// <returns>Feed URL.</returns>
        public Task<string> GetFeedURLAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("autoUpdaterGetFeedURLComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterGetFeedURLComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.ToString());
            });

            BridgeConnector.Socket.Emit("autoUpdaterGetFeedURL", guid);

            return taskCompletionSource.Task;
        }
    }
}
