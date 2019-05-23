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
};
