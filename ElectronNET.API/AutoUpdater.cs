using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Enable apps to automatically update themselves. Based on electron-updater.
    /// </summary>
    public sealed class AutoUpdater
    {
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
