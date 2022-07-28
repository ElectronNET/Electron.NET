"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('registerIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            event.preventDefault();
            electronSocket.emit(channel, [args]);
            event.returnValue = null;
        });
    });
    socket.on('registerIpcMainChannelWithId', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            event.preventDefault();
            let wcId = event.sender.id;
            let wc = electron_1.webContents.fromId(wcId);
            let bw = electron_1.BrowserWindow.fromWebContents(wc);
            if (bw) {
                electronSocket.emit(channel, { id: bw.id, wcId: wcId, args: [args] });
            }
            event.returnValue = null;
        });
    });
    socket.on('registerSyncIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            const x = socket;
            x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });
            event.preventDefault();
            electronSocket.emit(channel, [args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', (channel) => {
        electron_1.ipcMain.once(channel, (event, args) => {
            event.preventDefault();
            electronSocket.emit(channel, [args]);
            event.returnValue = null;
        });
    });
    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        electron_1.ipcMain.removeAllListeners(channel);
    });
    socket.on('sendToIpcRenderer', (browserWindowId, channel, ...data) => {
        const window = electron_1.BrowserWindow.fromId(browserWindowId);
        if (window) {
            window.webContents.send(channel, ...data);
        }
    });
    socket.on('sendToIpcRendererBrowserView', (id, channel, ...data) => {
        const browserViews = (global['browserViews'] = global['browserViews'] || []);
        let view = null;
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
//# sourceMappingURL=ipc.js.map