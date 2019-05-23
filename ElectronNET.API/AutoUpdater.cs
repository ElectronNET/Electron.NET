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
    }
}
