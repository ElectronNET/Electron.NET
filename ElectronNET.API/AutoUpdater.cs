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
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public sealed class AutoUpdater
    {
        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        public Task<bool> IsAutoDownloadEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.On<bool>("autoUpdater-autoDownload-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-autoDownload-get-reply");
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit("autoUpdater-autoDownload-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        public Task<bool> IsAutoInstallOnAppQuitEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.On<bool>("autoUpdater-autoInstallOnAppQuit-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-autoInstallOnAppQuit-get-reply");
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit("autoUpdater-autoInstallOnAppQuit-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// *GitHub provider only.* Whether to allow update to pre-release versions. 
        /// Defaults to "true" if application version contains prerelease components (e.g. "0.12.1-alpha.1", here "alpha" is a prerelease component), otherwise "false".
        /// 
        /// If "true", downgrade will be allowed("allowDowngrade" will be set to "true").
        /// </summary>
        public Task<bool> IsAllowPrereleaseEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.On<bool>("autoUpdater-allowPrerelease-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-allowPrerelease-get-reply");
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit("autoUpdater-allowPrerelease-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        public Task<bool> IsFullChangeLogEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.On<bool>("autoUpdater-fullChangelog-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-fullChangelog-get-reply");
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit("autoUpdater-fullChangelog-get");

            return taskCompletionSource.Task;
        }

        public Task<bool> IsAllowDowngradeEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.On<bool>("autoUpdater-allowDowngrade-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-allowDowngrade-get-reply");
                taskCompletionSource.SetResult(result);
            });

            BridgeConnector.Emit("autoUpdater-allowDowngrade-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        public bool AutoDownload
        {
            set
            {
                BridgeConnector.Emit("autoUpdater-autoDownload-set", value);
            }
        }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        public bool AutoInstallOnAppQuit
        {
            set
            {
                BridgeConnector.Emit("autoUpdater-autoInstallOnAppQuit-set", value);
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
            set
            {
                BridgeConnector.Emit("autoUpdater-allowPrerelease-set", value);
            }
        }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        public bool FullChangelog
        {
            set
            {
                BridgeConnector.Emit("autoUpdater-fullChangelog-set", value);
            }
        }

        /// <summary>
        /// Whether to allow version downgrade (when a user from the beta channel wants to go back to the stable channel).
        /// Taken in account only if channel differs (pre-release version component in terms of semantic versioning).
        /// Default is false.
        /// </summary>
        public bool AllowDowngrade
        {
            set
            {
                BridgeConnector.Emit("autoUpdater-allowDowngrade-set", value);
            }
        }

        /// <summary>
        /// For test only.
        /// </summary>
        public Task<string> GetUpdateConfigPathAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.On<string>("autoUpdater-updateConfigPath-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-updateConfigPath-get-reply");
                taskCompletionSource.SetResult(result.ToString());
            });

            BridgeConnector.Emit("autoUpdater-updateConfigPath-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The current application version
        /// </summary>
        public Task<SemVer> GetCurrentVersionAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<SemVer>();

            BridgeConnector.On<SemVer>("autoUpdater-currentVersion-get-reply", (version) =>
            {
                BridgeConnector.Off("autoUpdater-currentVersion-get-reply");
                taskCompletionSource.SetResult(version);
            });
            BridgeConnector.Emit("autoUpdater-currentVersion-get");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        public Task<string> GetChannelAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.On<string>("autoUpdater-channel-get-reply", (result) =>
            {
                BridgeConnector.Off("autoUpdater-channel-get-reply");
                taskCompletionSource.SetResult(result.ToString());
            });
            BridgeConnector.Emit("autoUpdater-channel-get");

            return taskCompletionSource.Task;
        }



        /// <summary>
        /// The request headers.
        /// </summary>
        public Task<Dictionary<string, string>> GetRequestHeadersAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Dictionary<string, string>>();
            BridgeConnector.On<Dictionary<string, string>>("autoUpdater-requestHeaders-get-reply", (headers) =>
            {
                BridgeConnector.Off("autoUpdater-requestHeaders-get-reply");
                taskCompletionSource.SetResult(headers);
            });
            BridgeConnector.Emit("autoUpdater-requestHeaders-get");
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The request headers.
        /// </summary>
        public Dictionary<string, string> RequestHeaders
        {
            set
            {
                BridgeConnector.Emit("autoUpdater-requestHeaders-set", JObject.FromObject(value, _jsonSerializer));
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
                    BridgeConnector.On<string>("autoUpdater-error" + GetHashCode(), (message) =>
                    {
                        _error(message.ToString());
                    });

                    BridgeConnector.Emit("register-autoUpdater-error-event", GetHashCode());
                }
                _error += value;
            }
            remove
            {
                _error -= value;

                if (_error == null)
                    BridgeConnector.Off("autoUpdater-error" + GetHashCode());
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
                    BridgeConnector.On("autoUpdater-checking-for-update" + GetHashCode(), () =>
                    {
                        _checkingForUpdate();
                    });

                    BridgeConnector.Emit("register-autoUpdater-checking-for-update-event", GetHashCode());
                }
                _checkingForUpdate += value;
            }
            remove
            {
                _checkingForUpdate -= value;

                if (_checkingForUpdate == null)
                    BridgeConnector.Off("autoUpdater-checking-for-update" + GetHashCode());
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
                    BridgeConnector.On<UpdateInfo>("autoUpdater-update-available" + GetHashCode(), (updateInfo) =>
                    {
                        _updateAvailable(updateInfo);
                    });

                    BridgeConnector.Emit("register-autoUpdater-update-available-event", GetHashCode());
                }
                _updateAvailable += value;
            }
            remove
            {
                _updateAvailable -= value;

                if (_updateAvailable == null)
                    BridgeConnector.Off("autoUpdater-update-available" + GetHashCode());
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
                    BridgeConnector.On<UpdateInfo>("autoUpdater-update-not-available" + GetHashCode(), (updateInfo) =>
                    {
                        _updateNotAvailable(updateInfo);
                    });

                    BridgeConnector.Emit("register-autoUpdater-update-not-available-event", GetHashCode());
                }
                _updateNotAvailable += value;
            }
            remove
            {
                _updateNotAvailable -= value;

                if (_updateNotAvailable == null)
                    BridgeConnector.Off("autoUpdater-update-not-available" + GetHashCode());
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
                    BridgeConnector.On<ProgressInfo>("autoUpdater-download-progress" + GetHashCode(), (progressInfo) =>
                    {
                        _downloadProgress(progressInfo);
                    });

                    BridgeConnector.Emit("register-autoUpdater-download-progress-event", GetHashCode());
                }
                _downloadProgress += value;
            }
            remove
            {
                _downloadProgress -= value;

                if (_downloadProgress == null)
                    BridgeConnector.Off("autoUpdater-download-progress" + GetHashCode());
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
                    BridgeConnector.On<UpdateInfo>("autoUpdater-update-downloaded" + GetHashCode(), (updateInfo) =>
                    {
                        _updateDownloaded(updateInfo);
                    });

                    BridgeConnector.Emit("register-autoUpdater-update-downloaded-event", GetHashCode());
                }
                _updateDownloaded += value;
            }
            remove
            {
                _updateDownloaded -= value;

                if (_updateDownloaded == null)
                    BridgeConnector.Off("autoUpdater-update-downloaded" + GetHashCode());
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

            BridgeConnector.On<UpdateCheckResult>("autoUpdaterCheckForUpdatesComplete" + guid, (updateCheckResult) =>
            {
                try
                {
                    BridgeConnector.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                    BridgeConnector.Off("autoUpdaterCheckForUpdatesError" + guid);
                    taskCompletionSource.SetResult(updateCheckResult);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.On<string>("autoUpdaterCheckForUpdatesError" + guid, (error) =>
            {
                BridgeConnector.Off("autoUpdaterCheckForUpdatesComplete" + guid);
                BridgeConnector.Off("autoUpdaterCheckForUpdatesError" + guid);
                string message = "An error occurred in CheckForUpdatesAsync";
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                    message = JsonConvert.SerializeObject(error);
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Emit("autoUpdaterCheckForUpdates", guid);

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

            BridgeConnector.On<UpdateCheckResult>("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid, (updateCheckResult) =>
            {
                try
                {
                    BridgeConnector.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                    BridgeConnector.Off("autoUpdaterCheckForUpdatesAndNotifyError" + guid);
                    if (updateCheckResult == null)
                        taskCompletionSource.SetResult(null);
                    else
                        taskCompletionSource.SetResult(updateCheckResult);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.On<string>("autoUpdaterCheckForUpdatesAndNotifyError" + guid, (error) =>
            {
                BridgeConnector.Off("autoUpdaterCheckForUpdatesAndNotifyComplete" + guid);
                BridgeConnector.Off("autoUpdaterCheckForUpdatesAndNotifyError" + guid);
                string message = "An error occurred in autoUpdaterCheckForUpdatesAndNotify";
                if (error != null)
                    message = JsonConvert.SerializeObject(error);
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Emit("autoUpdaterCheckForUpdatesAndNotify", guid);

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
            BridgeConnector.Emit("autoUpdaterQuitAndInstall", isSilent, isForceRunAfter);
        }

        /// <summary>
        /// Start downloading update manually. You can use this method if "AutoDownload" option is set to "false".
        /// </summary>
        /// <returns>Path to downloaded file.</returns>
        public Task<string> DownloadUpdateAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On<string>("autoUpdaterDownloadUpdateComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Off("autoUpdaterDownloadUpdateComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.ToString());
            });

            BridgeConnector.Emit("autoUpdaterDownloadUpdate", guid);

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

            BridgeConnector.On<string>("autoUpdaterGetFeedURLComplete" + guid, (downloadedPath) =>
            {
                BridgeConnector.Off("autoUpdaterGetFeedURLComplete" + guid);
                taskCompletionSource.SetResult(downloadedPath.ToString());
            });

            BridgeConnector.Emit("autoUpdaterGetFeedURL", guid);

            return taskCompletionSource.Task;
        }

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}
