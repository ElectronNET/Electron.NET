import { HubConnection  } from "@microsoft/signalr";
import { autoUpdater } from 'electron-updater';

export = (socket: HubConnection, app: Electron.App) => {

    socket.on('register-autoUpdater-error-event', (id) => {
        autoUpdater.on('error', (error) => {
            socket.invoke('AutoUpdaterOnError', id, error.message);
        });
    });

    socket.on('register-autoUpdater-checking-for-update-event', (id) => {
        autoUpdater.on('checking-for-update', () => {
            socket.invoke('AutoUpdaterOnCheckingForUpdate', id);
        });
    });

    socket.on('register-autoUpdater-update-available-event', (id) => {
        autoUpdater.on('update-available', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateAvailable', id, updateInfo);
        });
    });

    socket.on('register-autoUpdater-update-not-available-event', (id) => {
        autoUpdater.on('update-not-available', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateNotAvailable', id, updateInfo);
        });
    });

    socket.on('register-autoUpdater-download-progress-event', (id) => {
        autoUpdater.on('download-progress', (progressInfo) => {
            socket.invoke('AutoUpdaterOnDownloadProgress', id, progressInfo);
        });
    });

    socket.on('register-autoUpdater-update-downloaded-event', (id) => {
        autoUpdater.on('update-downloaded', (updateInfo) => {
            socket.invoke('AutoUpdaterOnUpdateDownloaded', id, updateInfo);
        });
    });

    // Properties *****

    socket.on('autoUpdater-autoDownload-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, autoUpdater.autoDownload);
    });

    socket.on('autoUpdater-autoDownload-set', (value) => {
        autoUpdater.autoDownload = value;
    });

    socket.on('autoUpdater-autoInstallOnAppQuit-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, autoUpdater.autoInstallOnAppQuit);
    });

    socket.on('autoUpdater-autoInstallOnAppQuit-set', (value) => {
        autoUpdater.autoInstallOnAppQuit = value;
    });

    socket.on('autoUpdater-allowPrerelease-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, autoUpdater.allowPrerelease);
    });

    socket.on('autoUpdater-allowPrerelease-set', (value) => {
        autoUpdater.allowPrerelease = value;
    });

    socket.on('autoUpdater-fullChangelog-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, autoUpdater.fullChangelog);
    });

    socket.on('autoUpdater-fullChangelog-set', (value) => {
        autoUpdater.fullChangelog = value;
    });

    socket.on('autoUpdater-allowDowngrade-get', (guid) => {
        socket.invoke('SendClientResponseBool', guid, autoUpdater.allowDowngrade);
    });

    socket.on('autoUpdater-allowDowngrade-set', (value) => {
        autoUpdater.allowDowngrade = value;
    });

    socket.on('autoUpdater-updateConfigPath-get', (guid) => {
        socket.invoke('SendClientResponseString', guid, autoUpdater.updateConfigPath || '');
    });

    socket.on('autoUpdater-updateConfigPath-set', (value) => {
        autoUpdater.updateConfigPath = value;
    });

    socket.on('autoUpdater-currentVersion-get', (guid) => {
        socket.invoke('SendClientResponseJObject', guid, autoUpdater.currentVersion);
    });

    socket.on('autoUpdater-channel-get', (guid) => {
        socket.invoke('SendClientResponseString', guid, autoUpdater.channel || '');
    });

    socket.on('autoUpdater-channel-set', (value) => {
        autoUpdater.channel = value;
    });

    socket.on('autoUpdater-requestHeaders-get', () => {
        socket.invoke('SendClientResponseJObject', autoUpdater.requestHeaders);
    });

    socket.on('autoUpdater-requestHeaders-set', (value) => {
        autoUpdater.requestHeaders = value;
    });

    socket.on('autoUpdaterCheckForUpdatesAndNotify', async (guid) => {
        autoUpdater.checkForUpdatesAndNotify().then((updateCheckResult) => {
            socket.invoke('SendClientResponseJObject', guid, updateCheckResult);
        }).catch((error) => {
            socket.invoke('SendClientResponseJObject', guid, error);
        });
    });

    socket.on('autoUpdaterCheckForUpdates', async (guid) => {
        autoUpdater.checkForUpdates().then((updateCheckResult) => {
            socket.invoke('SendClientResponseJObject', guid, updateCheckResult);
        }).catch((error) => {
            socket.invoke('SendClientResponseJObject', guid, error);
        });
    });

    socket.on('autoUpdaterQuitAndInstall', async (isSilent, isForceRunAfter) => {
        autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
    });

    socket.on('autoUpdaterDownloadUpdate', async (guid) => {
        const downloadedPath = await autoUpdater.downloadUpdate();
        socket.invoke('SendClientResponseString', guid, downloadedPath);
    });

    socket.on('autoUpdaterGetFeedURL', async (guid) => {
        const feedUrl = await autoUpdater.getFeedURL();
        socket.invoke('SendClientResponseString', guid, feedUrl || '');
    });
};
