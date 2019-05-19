import { autoUpdater } from 'electron-updater';
const path = require('path');
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;

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
