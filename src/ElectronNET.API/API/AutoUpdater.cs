using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public sealed class AutoUpdater: ApiBase
    {
        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.DashesLowerFirst;
        protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashedLower;

        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        public bool AutoDownload
        {
            get
            {
                return Task.Run(() => this.InvokeAsync<bool>()).Result;
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
                return Task.Run(() => this.InvokeAsync<bool>()).Result;
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
                return Task.Run(() => this.InvokeAsync<bool>()).Result;
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
                return Task.Run(() => this.InvokeAsync<bool>()).Result;
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
                return Task.Run(() => this.InvokeAsync<bool>()).Result;
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
                return Task.Run(() => this.InvokeAsync<string>()).Result;
            }
        }

        /// <summary>
        /// The current application version
        /// </summary>
        public Task<SemVer> CurrentVersionAsync
        {
            get
            {
                return Task.Run(() => this.InvokeAsync<SemVer>());
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
                return Task.Run(() => this.InvokeAsync<string>());
            }
        }

        /// <summary>
        /// Set the update channel. Not applicable for GitHub. 
        /// </summary>
        public string SetChannel
        {
            set
            {
                BridgeConnector.Socket.Emit("autoUpdater-channel-set", value);
            }
        }


        /// <summary>
        /// The request headers.
        /// </summary>
        public Task<Dictionary<string, string>> RequestHeadersAsync
        {
            get
            {
                return Task.Run(() => this.InvokeAsync<Dictionary<string, string>>());
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
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted when checking if an update has started.
        /// </summary>
        public event Action OnCheckingForUpdate
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted when there is an available update. 
        /// The update is downloaded automatically if AutoDownload is true.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateAvailable
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted when there is no available update.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateNotAvailable
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted on download progress.
        /// </summary>
        public event Action<ProgressInfo> OnDownloadProgress
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted on download complete.
        /// </summary>
        public event Action<UpdateInfo> OnUpdateDownloaded
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

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

            BridgeConnector.Socket.Once<UpdateCheckResult>("autoUpdater-checkForUpdates-completed" + guid, (result) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdater-checkForUpdatesError" + guid);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.Once<string>("autoUpdater-checkForUpdatesError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdater-checkForUpdates-completed" + guid);
                string message = "An error occurred in CheckForUpdatesAsync";
                if (!string.IsNullOrEmpty(result)) message = result;
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Socket.Emit("autoUpdater-checkForUpdates", guid);

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

            BridgeConnector.Socket.Once<UpdateCheckResult>("autoUpdater-checkForUpdatesAndNotify-completed" + guid, (result) =>
            {
                try
                {
                    BridgeConnector.Socket.Off("autoUpdater-checkForUpdatesAndNotifyError" + guid);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            BridgeConnector.Socket.Once<string>("autoUpdater-checkForUpdatesAndNotifyError" + guid, (result) =>
            {
                BridgeConnector.Socket.Off("autoUpdater-checkForUpdatesAndNotify-completed" + guid);
                string message = "An error occurred in CheckForUpdatesAndNotifyAsync";
                if (!string.IsNullOrEmpty(result)) message = result;
                taskCompletionSource.SetException(new Exception(message));
            });

            BridgeConnector.Socket.Emit("autoUpdater-checkForUpdatesAndNotify", guid);

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
            BridgeConnector.Socket.Emit("autoUpdater-quitAndInstall", isSilent, isForceRunAfter);
        }

        /// <summary>
        /// Start downloading update manually. You can use this method if "AutoDownload" option is set to "false".
        /// </summary>
        /// <returns>Path to downloaded file.</returns>
        public Task<string> DownloadUpdateAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<string>("autoUpdater-downloadUpdate-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("autoUpdater-downloadUpdate", guid);

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

            BridgeConnector.Socket.Once<string>("autoUpdater-getFeedURL-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("autoUpdater-getFeedURL", guid);

            return tcs.Task;
        }


    }
}


