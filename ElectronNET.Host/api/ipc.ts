import { ipcMain, BrowserWindow, BrowserView } from 'electron';
import { Socket } from 'net';
let electronSocket;

export = (socket: Socket) => {
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

    socket.on('sendToIpcRendererBrowserView', (id, channel, ...data) => {
        const browserViews: BrowserView[] = (global['browserViews'] = global['browserViews'] || []) as BrowserView[];
        let view: BrowserView = null;
        for (let i = 0; i < browserViews.length; i++) {
            if (browserViews[i]['id'] === id) {
                view = browserViews[i];
                break;
            }
        }

        if (view) {
            view.webContents.send(channel, ...data);
        }
    });
};
