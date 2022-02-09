import {Socket} from 'net';
import {autoUpdater} from 'electron-updater';
import {BrowserWindow} from 'electron';

let electronSocket;

export = (socket: Socket, app: Electron.App) => {
    electronSocket = socket;

    socket.on('register-autoUpdater-error-event', (id) => {
        autoUpdater.on('error', (error) => {
            electronSocket.emit('autoUpdater-error' + id, error.message);
        });
    });

    socket.on('register-autoUpdater-checking-for-update-event', (id) => {
        autoUpdater.on('checking-for-update', () => {
            electronSocket.emit('autoUpdater-checking-for-update' + id);
        });
    });

    socket.on('register-autoUpdater-update-available-event', (id) => {
        autoUpdater.on('update-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-available' + id, updateInfo);
        });
    });

    socket.on('register-autoUpdater-update-not-available-event', (id) => {
        autoUpdater.on('update-not-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-not-available' + id, updateInfo);
        });
    });

    socket.on('register-autoUpdater-download-progress-event', (id) => {
        autoUpdater.on('download-progress', (progressInfo) => {
            electronSocket.emit('autoUpdater-download-progress' + id, progressInfo);
        });
    });

    socket.on('register-autoUpdater-update-downloaded-event', (id) => {
        autoUpdater.on('update-downloaded', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-downloaded' + id, updateInfo);
        });
    });

    // Properties *****

    socket.on('autoUpdater-autoDownload-get', () => {
        electronSocket.emit('autoUpdater-autoDownload-get-reply', autoUpdater.autoDownload);
    });

    socket.on('autoUpdater-autoDownload-set', (value) => {
        autoUpdater.autoDownload = value;
    });

    socket.on('autoUpdater-autoInstallOnAppQuit-get', () => {
        electronSocket.emit('autoUpdater-autoInstallOnAppQuit-get-reply', autoUpdater.autoInstallOnAppQuit);
    });

    socket.on('autoUpdater-autoInstallOnAppQuit-set', (value) => {
        autoUpdater.autoInstallOnAppQuit = value;
    });

    socket.on('autoUpdater-allowPrerelease-get', () => {
        electronSocket.emit('autoUpdater-allowPrerelease-get-reply', autoUpdater.allowPrerelease);
    });

    socket.on('autoUpdater-allowPrerelease-set', (value) => {
        autoUpdater.allowPrerelease = value;
    });

    socket.on('autoUpdater-fullChangelog-get', () => {
        electronSocket.emit('autoUpdater-fullChangelog-get-reply', autoUpdater.fullChangelog);
    });

    socket.on('autoUpdater-fullChangelog-set', (value) => {
        autoUpdater.fullChangelog = value;
    });

    socket.on('autoUpdater-allowDowngrade-get', () => {
        electronSocket.emit('autoUpdater-allowDowngrade-get-reply', autoUpdater.allowDowngrade);
    });

    socket.on('autoUpdater-allowDowngrade-set', (value) => {
        autoUpdater.allowDowngrade = value;
    });

    socket.on('autoUpdater-updateConfigPath-get', () => {
        electronSocket.emit('autoUpdater-updateConfigPath-get-reply', autoUpdater.updateConfigPath || '');
    });

    socket.on('autoUpdater-updateConfigPath-set', (value) => {
        autoUpdater.updateConfigPath = value;
    });

    socket.on('autoUpdater-currentVersion-get', () => {
        electronSocket.emit('autoUpdater-currentVersion-get-reply', autoUpdater.currentVersion);
    });

    socket.on('autoUpdater-channel-get', () => {
        electronSocket.emit('autoUpdater-channel-get-reply', autoUpdater.channel || '');
    });

    socket.on('autoUpdater-channel-set', (value) => {
        autoUpdater.channel = value;
    });

    socket.on('autoUpdater-requestHeaders-get', () => {
        electronSocket.emit('autoUpdater-requestHeaders-get-reply', autoUpdater.requestHeaders);
    });

    socket.on('autoUpdater-requestHeaders-set', (value) => {
        autoUpdater.requestHeaders = value;
    });

    socket.on('autoUpdaterCheckForUpdatesAndNotify', async (guid) => {
        autoUpdater.checkForUpdatesAndNotify().then((updateCheckResult) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyComplete' + guid, updateCheckResult);
        }).catch((error) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyError' + guid, error);
        });
    });

    socket.on('autoUpdaterCheckForUpdates', async (guid) => {
        autoUpdater.checkForUpdates().then((updateCheckResult) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesComplete' + guid, updateCheckResult);
        }).catch((error) => {
            electronSocket.emit('autoUpdaterCheckForUpdatesError' + guid, error);
        });
    });

    socket.on('autoUpdaterQuitAndInstall', async (isSilent, isForceRunAfter) => {
        console.log('running autoUpdaterQuitAndInstall');

        app.removeAllListeners("window-all-closed");
        const windows = BrowserWindow.getAllWindows();
        if (windows && windows.length) {
            windows.forEach(w => {
                try {
                    w.removeAllListeners('close');
                    w.removeAllListeners('closed');
                    w.destroy();
                } catch {
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
            autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
        }, 100);
    });

    socket.on('autoUpdaterDownloadUpdate', async (guid) => {
        const downloadedPath = await autoUpdater.downloadUpdate();
        electronSocket.emit('autoUpdaterDownloadUpdateComplete' + guid, downloadedPath);
    });

    socket.on('autoUpdaterGetFeedURL', async (guid) => {
        const feedUrl = await autoUpdater.getFeedURL();
        electronSocket.emit('autoUpdaterGetFeedURLComplete' + guid, feedUrl || '');
    });
};
