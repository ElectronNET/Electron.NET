import { autoUpdater } from 'electron-updater';
const path = require('path');
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;

    // Events ********

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

    socket.on('autoUpdater-channel-get', () => {
        electronSocket.emit('autoUpdater-channel-get-reply', autoUpdater.channel || '');
    });

    socket.on('autoUpdater-channel-set', (value) => {
        autoUpdater.channel = value;
    });

    // Methods ********

    socket.on('autoUpdaterCheckForUpdatesAndNotify', async (guid) => {
        const updateCheckResult = await autoUpdater.checkForUpdatesAndNotify();
        electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyComplete' + guid, updateCheckResult);
    });

    socket.on('autoUpdaterCheckForUpdates', async (guid) => {
        // autoUpdater.updateConfigPath = path.join(__dirname, 'dev-app-update.yml');
        const updateCheckResult = await autoUpdater.checkForUpdates();
        electronSocket.emit('autoUpdaterCheckForUpdatesComplete' + guid, updateCheckResult);
    });

    socket.on('autoUpdaterQuitAndInstall', async (isSilent, isForceRunAfter) => {
        autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
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
