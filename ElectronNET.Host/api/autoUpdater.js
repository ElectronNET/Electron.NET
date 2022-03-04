"use strict";
const electron_updater_1 = require("electron-updater");
const electron_1 = require("electron");
module.exports = (socket, app) => {
    socket.on('register-autoUpdater-error-event', (id) => {
        electron_updater_1.autoUpdater.on('error', (error) => {
            socket.invoke('AutoUpdaterOnError', id, error.message);
        });
    });
    socket.on('register-autoUpdater-checking-for-update-event', (id) => {
        electron_updater_1.autoUpdater.on('checking-for-update', () => {
            socket.invoke('AutoUpdaterOnCheckingForUpdate', id);
        });
    });
    socket.on('register-autoUpdater-update-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-available', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateAvailable', id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-update-not-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-not-available', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateNotAvailable', id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-download-progress-event', (id) => {
        electron_updater_1.autoUpdater.on('download-progress', (progressInfo) => {
            socket.invoke('AutoUpdaterOnDownloadProgress', id, progressInfo);
        });
    });
    socket.on('register-autoUpdater-update-downloaded-event', (id) => {
        electron_updater_1.autoUpdater.on('update-downloaded', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateDownloaded', id, updateInfo);
        });
    });
    // Properties *****
    socket.on('autoUpdater-autoDownload-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, electron_updater_1.autoUpdater.autoDownload);
    });
    socket.on('autoUpdater-autoDownload-set', (value) => {
        electron_updater_1.autoUpdater.autoDownload = value;
    });
    socket.on('autoUpdater-autoInstallOnAppQuit-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, electron_updater_1.autoUpdater.autoInstallOnAppQuit);
    });
    socket.on('autoUpdater-autoInstallOnAppQuit-set', (value) => {
        electron_updater_1.autoUpdater.autoInstallOnAppQuit = value;
    });
    socket.on('autoUpdater-allowPrerelease-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, electron_updater_1.autoUpdater.allowPrerelease);
    });
    socket.on('autoUpdater-allowPrerelease-set', (value) => {
        electron_updater_1.autoUpdater.allowPrerelease = value;
    });
    socket.on('autoUpdater-fullChangelog-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, electron_updater_1.autoUpdater.fullChangelog);
    });
    socket.on('autoUpdater-fullChangelog-set', (value) => {
        electron_updater_1.autoUpdater.fullChangelog = value;
    });
    socket.on('autoUpdater-allowDowngrade-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, electron_updater_1.autoUpdater.allowDowngrade);
    });
    socket.on('autoUpdater-allowDowngrade-set', (value) => {
        electron_updater_1.autoUpdater.allowDowngrade = value;
    });
    socket.on('autoUpdater-updateConfigPath-get', (guid) => {
        socket.invoke('SendClientResponseString', guid, electron_updater_1.autoUpdater.updateConfigPath || '');
    });
    socket.on('autoUpdater-updateConfigPath-set', (value) => {
        electron_updater_1.autoUpdater.updateConfigPath = value;
    });
    socket.on('autoUpdater-currentVersion-get', (guid) => {
        socket.invoke('SendClientResponseJObject', guid, electron_updater_1.autoUpdater.currentVersion);
    });
    socket.on('autoUpdater-channel-get', (guid) => {
        socket.invoke('SendClientResponseString', guid, electron_updater_1.autoUpdater.channel || '');
    });
    socket.on('autoUpdater-channel-set', (value) => {
        electron_updater_1.autoUpdater.channel = value;
    });
    socket.on('autoUpdater-requestHeaders-get', () => {
        socket.invoke('SendClientResponseJObject', electron_updater_1.autoUpdater.requestHeaders);
    });
    socket.on('autoUpdater-requestHeaders-set', (value) => {
        electron_updater_1.autoUpdater.requestHeaders = value;
    });
    socket.on('autoUpdaterCheckForUpdatesAndNotify', async (guid) => {
        electron_updater_1.autoUpdater.checkForUpdatesAndNotify().then((updateCheckResult) => {
            socket.invoke('SendClientResponseJObject', guid, updateCheckResult);
        }).catch((error) => {
            socket.invoke('SendClientResponseJObject', guid, error);
        });
    });
    socket.on('autoUpdaterCheckForUpdates', async (guid) => {
        electron_updater_1.autoUpdater.checkForUpdates().then((updateCheckResult) => {
            socket.invoke('SendClientResponseJObject', guid, updateCheckResult);
        }).catch((error) => {
            socket.invoke('SendClientResponseJObject', guid, error);
        });
    });
    socket.on('autoUpdaterQuitAndInstall', async (isSilent, isForceRunAfter) => {
        console.log('running autoUpdaterQuitAndInstall');
        app.removeAllListeners("window-all-closed");
        const windows = electron_1.BrowserWindow.getAllWindows();
        if (windows && windows.length) {
            windows.forEach(w => {
                try {
                    w.removeAllListeners('close');
                    w.removeAllListeners('closed');
                    w.destroy();
                }
                catch {
                    //ignore, probably already destroyed
                }
            });
        }
        //The call to quitAndInstall needs to happen after the windows 
        //get a chance to close and release resources, so it must be done on a timeout
        setTimeout(() => {
            console.log('running autoUpdater.quitAndInstall');
            console.log('isSilent:' + isSilent);
            console.log('isForceRunAfter:' + isForceRunAfter);
            electron_updater_1.autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
        }, 100);
    });
    socket.on('autoUpdaterDownloadUpdate', async (guid) => {
        const downloadedPath = await electron_updater_1.autoUpdater.downloadUpdate();
        socket.invoke('SendClientResponseString', guid, downloadedPath);
    });
    socket.on('autoUpdaterGetFeedURL', async (guid) => {
        const feedUrl = await electron_updater_1.autoUpdater.getFeedURL();
        socket.invoke('SendClientResponseString', guid, feedUrl || '');
    });
};
//# sourceMappingURL=autoUpdater.js.map