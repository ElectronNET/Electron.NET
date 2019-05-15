import { ipcMain, BrowserWindow } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('registerIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerSyncIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            const x = <any>socket;
            x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });

            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerOnceIpcMainChannel', (channel) => {
        ipcMain.once(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        ipcMain.removeAllListeners(channel);
    });

    socket.on('sendToIpcRenderer', (browserWindow, channel, ...data) => {
        const window = BrowserWindow.fromId(browserWindow.id);

        if (window) {
            window.webContents.send(channel, ...data);
        }
    });
};
