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
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-autoDownload-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-autoDownload-get-reply");
                        taskCompletionSource.SetResult(result.GetBoolean());
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
                return Task.Run(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-autoInstallOnAppQuit-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-autoInstallOnAppQuit-get-reply");
                        taskCompletionSource.SetResult(result.GetBoolean());
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
                return Task.Run(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-allowPrerelease-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-allowPrerelease-get-reply");
                        taskCompletionSource.SetResult(result.GetBoolean());
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
                return Task.Run(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-fullChangelog-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-fullChangelog-get-reply");
                        taskCompletionSource.SetResult(result.GetBoolean());
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
                return Task.Run(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-allowDowngrade-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-allowDowngrade-get-reply");
                        taskCompletionSource.SetResult(result.GetBoolean());
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
                return Task.Run(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-updateConfigPath-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-updateConfigPath-get-reply");
                        taskCompletionSource.SetResult(result.GetString());
                    });

                    BridgeConnector.Socket.Emit("autoUpdater-updateConfigPath-get");

                    return taskCompletionSource.Task;
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
                    var taskCompletionSource = new TaskCompletionSource<SemVer>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-currentVersion-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-currentVersion-get-reply");
                        var version = result.Deserialize(ElectronJsonContext.Default.SemVer);
                        taskCompletionSource.SetResult(version);
                    });
                    BridgeConnector.Socket.Emit("autoUpdater-currentVersion-get");

                    return taskCompletionSource.Task;
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
                    var taskCompletionSource = new TaskCompletionSource<string>();

                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-channel-get-reply", (result) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-channel-get-reply");
                        taskCompletionSource.SetResult(result.GetString());
                    });
                    BridgeConnector.Socket.Emit("autoUpdater-channel-get");

                    return taskCompletionSource.Task;
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
                    var taskCompletionSource = new TaskCompletionSource<Dictionary<string, string>>();
                    BridgeConnector.Socket.On<JsonElement>("autoUpdater-requestHeaders-get-reply", (headers) =>
                    {
                        BridgeConnector.Socket.Off("autoUpdater-requestHeaders-get-reply");
                        var result = headers.Deserialize<Dictionary<string, string>>(ElectronJson.Options);
                        taskCompletionSource.SetResult(result);
                    });
                    BridgeConnector.Socket.Emit("autoUpdater-requestHeaders-get");
                    return taskCompletionSource.Task;
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

            BridgeConnector.Socket.On<JsonElement>("autoUpdaterCheckForUpdatesComplete" + guid, (updateCheckResult) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesError" + guid);
                    taskCompletionSource.SetResult(updateCheckResult.Deserialize(ElectronJsonContext.Default.UpdateCheckResult));
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.On<JsonElement>("autoUpdaterCheckForUpdatesError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesError" + guid);
                string message = "An error occurred in CheckForUpdatesAsync";
                var error = result.GetString();
                if (!string.IsNullOrEmpty(error)) message = error;
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

            BridgeConnector.Socket.On<JsonElement>("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid, (updateCheckResult) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                    BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyError" + guid);
                    taskCompletionSource.SetResult(updateCheckResult.Deserialize(ElectronJsonContext.Default.UpdateCheckResult));
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.On<JsonElement>("autoUpdaterCheckForUpdatesAndNotifyError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                BridgeConnector.Socket.Off("autoUpdaterCheckForUpdatesAndNotifyError" + guid);
                string message = "An error occurred in autoUpdaterCheckForUpdatesAndNotify";
                var error = result.GetString();
                if (!string.IsNullOrEmpty(error)) message = error;
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
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On<JsonElement>("autoUpdaterDownloadUpdateComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterDownloadUpdateComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.GetString());
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

            BridgeConnector.Socket.On<JsonElement>("autoUpdaterGetFeedURLComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Socket.Off("autoUpdaterGetFeedURLComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.GetString());
            });

            BridgeConnector.Socket.Emit("autoUpdaterGetFeedURL", guid);

            return taskCompletionSource.Task;
        }


    }
}


