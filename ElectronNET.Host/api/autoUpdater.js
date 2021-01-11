"use strict";
const electron_updater_1 = require("electron-updater");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('register-autoUpdater-error-event', (id) => {
        electron_updater_1.autoUpdater.on('error', (error) => {
            electronSocket.emit('autoUpdater-error' + id, error.message);
        });
    });
    socket.on('register-autoUpdater-checking-for-update-event', (id) => {
        electron_updater_1.autoUpdater.on('checking-for-update', () => {
            electronSocket.emit('autoUpdater-checking-for-update' + id);
        });
    });
    socket.on('register-autoUpdater-update-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-available' + id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-update-not-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-not-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-not-available' + id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-download-progress-event', (id) => {
        electron_updater_1.autoUpdater.on('download-progress', (progressInfo) => {
            electronSocket.emit('autoUpdater-download-progress' + id, progressInfo);
        });
    });
    socket.on('register-autoUpdater-update-downloaded-event', (id) => {
        electron_updater_1.autoUpdater.on('update-downloaded', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-downloaded' + id, updateInfo);
        });
    });
    // Properties *****
    socket.on('autoUpdater-autoDownload-get', () => {
        electronSocket.emit('autoUpdater-autoDownload-get-reply', electron_updater_1.autoUpdater.autoDownload);
    });
    socket.on('autoUpdater-autoDownload-set', (value) => {
        electron_updater_1.autoUpdater.autoDownload = value;
    });
    socket.on('autoUpdater-autoInstallOnAppQuit-get', () => {
        electronSocket.emit('autoUpdater-autoInstallOnAppQuit-get-reply', electron_updater_1.autoUpdater.autoInstallOnAppQuit);
    });
    socket.on('autoUpdater-autoInstallOnAppQuit-set', (value) => {
        electron_updater_1.autoUpdater.autoInstallOnAppQuit = value;
    });
    socket.on('autoUpdater-allowPrerelease-get', () => {
        electronSocket.emit('autoUpdater-allowPrerelease-get-reply', electron_updater_1.autoUpdater.allowPrerelease);
    });
    socket.on('autoUpdater-allowPrerelease-set', (value) => {
        electron_updater_1.autoUpdater.allowPrerelease = value;
    });
    socket.on('autoUpdater-fullChangelog-get', () => {
        electronSocket.emit('autoUpdater-fullChangelog-get-reply', electron_updater_1.autoUpdater.fullChangelog);
    });
    socket.on('autoUpdater-fullChangelog-set', (value) => {
        electron_updater_1.autoUpdater.fullChangelog = value;
    });
    socket.on('autoUpdater-allowDowngrade-get', () => {
        electronSocket.emit('autoUpdater-allowDowngrade-get-reply', electron_updater_1.autoUpdater.allowDowngrade);
    });
    socket.on('autoUpdater-allowDowngrade-set', (value) => {
        electron_updater_1.autoUpdater.allowDowngrade = value;
    });
    socket.on('autoUpdater-updateConfigPath-get', () => {
        electronSocket.emit('autoUpdater-updateConfigPath-get-reply', electron_updater_1.autoUpdater.updateConfigPath || '');
    });
    socket.on('autoUpdater-updateConfigPath-set', (value) => {
        electron_updater_1.autoUpdater.updateConfigPath = value;
    });
    socket.on('autoUpdater-currentVersion-get', () => {
        electronSocket.emit('autoUpdater-currentVersion-get-reply', electron_updater_1.autoUpdater.currentVersion);
    });
    socket.on('autoUpdater-channel-get', () => {
        electronSocket.emit('autoUpdater-channel-get-reply', electron_updater_1.autoUpdater.channel || '');
    });
    socket.on('autoUpdater-channel-set', (value) => {
        electron_updater_1.autoUpdater.channel = value;
    });
    socket.on('autoUpdater-requestHeaders-get', () => {
        electronSocket.emit('autoUpdater-requestHeaders-get-reply', electron_updater_1.autoUpdater.requestHeaders);
    });
    socket.on('autoUpdater-requestHeaders-set', (value) => {
        electron_updater_1.autoUpdater.requestHeaders = value;
    });
    socket.on('autoUpdaterCheckForUpdatesAndNotify', async (guid) => {
        electron_updater_1.autoUpdater.checkForUpdatesAndNotify().then((updateCheckResult) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyComplete' + guid, updateCheckResult);
        }).catch((error) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyError' + guid, error);
        });
    });
    socket.on('autoUpdaterCheckForUpdates', async (guid) => {
        electron_updater_1.autoUpdater.checkForUpdates().then((updateCheckResult) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesComplete' + guid, updateCheckResult);
        }).catch((error) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesError' + guid, error);
        });
    });
    socket.on('autoUpdaterQuitAndInstall', async (isSilent, isForceRunAfter) => {
        electron_updater_1.autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
    });
    socket.on('autoUpdaterDownloadUpdate', async (guid) => {
        const downloadedPath = await electron_updater_1.autoUpdater.downloadUpdate();
        electronSocket.emit('autoUpdaterDownloadUpdateComplete' + guid, downloadedPath);
    });
    socket.on('autoUpdaterGetFeedURL', async (guid) => {
        const feedUrl = await electron_updater_1.autoUpdater.getFeedURL();
        electronSocket.emit('autoUpdaterGetFeedURLComplete' + guid, feedUrl || '');
    });
};
//# sourceMappingURL=autoUpdater.js.map