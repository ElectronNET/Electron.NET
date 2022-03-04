using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
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
        public async Task<bool> IsAutoDownloadEnabledAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("autoUpdater-autoDownload-get");
        }

        /// <summary>
        /// Whether to automatically install a downloaded update on app quit (if `QuitAndInstall` was not called before).
        /// 
        /// Applicable only on Windows and Linux.
        /// </summary>
        public async Task<bool> IsAutoInstallOnAppQuitEnabledAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("autoUpdater-autoInstallOnAppQuit-get");
        }

        /// <summary>
        /// *GitHub provider only.* Whether to allow update to pre-release versions. 
        /// Defaults to "true" if application version contains prerelease components (e.g. "0.12.1-alpha.1", here "alpha" is a prerelease component), otherwise "false".
        /// 
        /// If "true", downgrade will be allowed("allowDowngrade" will be set to "true").
        /// </summary>
        public async Task<bool> IsAllowPrereleaseEnabledAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("autoUpdater-allowPrerelease-get");
        }

        /// <summary>
        /// *GitHub provider only.* 
        /// Get all release notes (from current version to latest), not just the latest (Default is false).
        /// </summary>
        public async Task<bool> IsFullChangeLogEnabledAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("autoUpdater-fullChangelog-get");
        }

        public async Task<bool> IsAllowDowngradeEnabledAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("autoUpdater-allowDowngrade-get");
        }
        

        /// <summary>
        /// Whether to automatically download an update when it is found. (Default is true)
        /// </summary>
        public bool AutoDownload
        {
            set
            {
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-autoDownload-set", value);
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
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-autoInstallOnAppQuit-set", value);
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
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-allowPrerelease-set", value);
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
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-fullChangelog-set", value);
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
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-allowDowngrade-set", value);
            }
        }

        /// <summary>
        /// For test only.
        /// </summary>
        public async Task<string> GetUpdateConfigPathAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("autoUpdater-updateConfigPath-get");
        }
        
        /// <summary>
        /// The current application version
        /// </summary>
        public async Task<SemVer> GetCurrentVersionAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultString("autoUpdater-updateConcurrentVersionfigPath-get");
            SemVer version = ((JObject)signalrResult).ToObject<SemVer>();
            return version;
        }


        /// <summary>
        /// Get the update channel. Not applicable for GitHub. 
        /// Doesn’t return channel from the update configuration, only if was previously set.
        /// </summary>
        public async Task<string> GetChannelAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("autoUpdater-channel-get");
        }

        /// <summary>
        /// The request headers.
        /// </summary>
        public async Task<Dictionary<string, string>> GetRequestHeadersAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("autoUpdater-requestHeaders-get");
            Dictionary<string, string> result = signalrResult.ToObject<Dictionary<string, string>>();
            return result;
        }

        /// <summary>
        /// The request headers.
        /// </summary>
        public Dictionary<string, string> RequestHeaders
        {
            set
            {
                Electron.SignalrElectron.Clients.All.SendAsync("autoUpdater-requestHeaders-set", JObject.FromObject(value, _jsonSerializer));
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-error-event", GetHashCode());

                }
                _error += value;
            }
            remove
            {
                _error -= value;
            }
        }

        public void TriggerOnError(string error)
        {
            _error(error);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-checking-for-update-event", GetHashCode());

                }
                _checkingForUpdate += value;
            }
            remove
            {
                _checkingForUpdate -= value;
            }
        }

        public void TriggerOnCheckingForUpdate()
        {
            _checkingForUpdate();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-update-available-event", GetHashCode());
                }
                _updateAvailable += value;
            }
            remove
            {
                _updateAvailable -= value;
            }
        }

        public void TriggerOnUpdateAvailable(JObject jobject)
        {
            _updateAvailable(JObject.Parse(jobject.ToString()).ToObject<UpdateInfo>());
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-update-not-available-event", GetHashCode());
                }
                _updateNotAvailable += value;
            }
            remove
            {
                _updateNotAvailable -= value;
            }
        }

        public void TriggerOnUpdateNotAvailable(JObject jobject)
        {
            _updateNotAvailable(JObject.Parse(jobject.ToString()).ToObject<UpdateInfo>());
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-download-progress-event", GetHashCode());
                }
                _downloadProgress += value;
            }
            remove
            {
                _downloadProgress -= value;
            }
        }

        public void TriggerOnDownloadProgress(JObject jobject)
        {
            _downloadProgress(JObject.Parse(jobject.ToString()).ToObject<ProgressInfo>());
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-autoUpdater-update-downloaded-event", GetHashCode());
                }
                _updateDownloaded += value;
            }
            remove
            {
                _updateDownloaded -= value;
            }
        }

        public void TriggerOnUpdateDownloaded(JObject jobject)
        {
            _updateDownloaded(JObject.Parse(jobject.ToString()).ToObject<UpdateInfo>());
        }

        private event Action<UpdateInfo> _updateDownloaded;

        private static AutoUpdater _autoUpdater;
        private static readonly object _syncRoot = new();

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
        public async Task<UpdateCheckResult> CheckForUpdatesAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<UpdateCheckResult>();
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("autoUpdaterCheckForUpdates");

            if (signalrResult["errno"] != null)
            {
                string message = "An error occurred in CheckForUpdatesAsync";
                message = JsonConvert.SerializeObject(signalrResult);
                taskCompletionSource.SetException(new Exception(message));
            } 
            else
            {
                try
                {
                    var updateCheckResult = signalrResult.ToObject<UpdateCheckResult>();
                    taskCompletionSource.SetResult(updateCheckResult);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }

            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Asks the server whether there is an update.
        /// 
        /// This will immediately download an update, then install when the app quits.
        /// </summary>
        /// <returns></returns>
        public async Task<UpdateCheckResult> CheckForUpdatesAndNotifyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<UpdateCheckResult>();
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("autoUpdaterCheckForUpdatesAndNotify");

            if (signalrResult["errno"] != null)
            {
                string message = "An error occurred in CheckForUpdatesAsync";
                message = JsonConvert.SerializeObject(signalrResult);
                taskCompletionSource.SetException(new Exception(message));
            }
            else
            {
                try
                {
                    var updateCheckResult = signalrResult.ToObject<UpdateCheckResult>();
                    taskCompletionSource.SetResult(updateCheckResult);
                        taskCompletionSource.SetResult(JObject.Parse(updateCheckResult.ToString()).ToObject<UpdateCheckResult>());
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }

            return await taskCompletionSource.Task;
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
        public async void QuitAndInstall(bool isSilent = false, bool isForceRunAfter = false)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("prepare-for-update", isSilent, isForceRunAfter);
            await Electron.SignalrElectron.Clients.All.SendAsync("autoUpdaterQuitAndInstall", isSilent, isForceRunAfter);
        }

        /// <summary>
        /// Start downloading update manually. You can use this method if "AutoDownload" option is set to "false".
        /// </summary>
        /// <returns>Path to downloaded file.</returns>
        public async Task<string> DownloadUpdateAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("autoUpdaterDownloadUpdate");
        }

        /// <summary>
        /// Feed URL.
        /// </summary>
        /// <returns>Feed URL.</returns>
        public async Task<string> GetFeedURLAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("autoUpdaterGetFeedURL");
        }

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}
