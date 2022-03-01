"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('registerIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            socket.invoke("IpcOnChannel", channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerSyncIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            //const x = <any>socket;
            //x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });
            socket.invoke("IpcOnChannel", channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', (guid, channel) => {
        electron_1.ipcMain.once(channel, (event, args) => {
            socket.invoke("SendClientResponseJArray", guid, [event.preventDefault(), args]);
        });
    });
    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        electron_1.ipcMain.removeAllListeners(channel);
    });
    socket.on('sendToIpcRenderer', (browserWindow, channel, ...data) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
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