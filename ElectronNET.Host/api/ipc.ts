const { ipcMain } = require('electron');

module.exports = (socket: SocketIO.Server, window) => {
    socket.on('registerIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerOnceIpcMainChannel', (channel) => {
        ipcMain.once(channel, (event, args) => {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        ipcMain.removeAllListeners(channel);
    });

    socket.on('sendToIpcRenderer', (channel, ...data) => {
        if (window) {
            window.webContents.send(channel, data);
        }
    });
}