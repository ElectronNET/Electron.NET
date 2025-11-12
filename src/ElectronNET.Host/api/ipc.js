"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('registerIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerSyncIpcMainChannel', (channel) => {
        electron_1.ipcMain.on(channel, (event, args) => {
            const x = socket;
            x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', (channel) => {
        electron_1.ipcMain.once(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        electron_1.ipcMain.removeAllListeners(channel);
    });
    socket.on('sendToIpcRenderer', (browserWindow, channel, data) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        if (window) {
            window.webContents.send(channel, ...data);
        }
    });
    socket.on('sendToIpcRendererBrowserView', (id, channel, data) => {
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
    // Integration helpers: programmatically click menu items from renderer tests
    electron_1.ipcMain.on('integration-click-application-menu', (event, id) => {
        try {
            const menu = electron_1.Menu.getApplicationMenu();
            const mi = menu ? menu.getMenuItemById(id) : null;
            if (mi && typeof mi.click === 'function') {
                const bw = electron_1.BrowserWindow.fromWebContents(event.sender);
                mi.click(undefined, bw, undefined);
            }
        }
        catch { /* ignore */ }
    });
    electron_1.ipcMain.on('integration-click-context-menu', (event, windowId, id) => {
        var _a, _b;
        try {
            const entries = global['contextMenuItems'] || [];
            const entry = entries.find((x) => x.browserWindowId === windowId);
            const mi = (_b = (_a = entry === null || entry === void 0 ? void 0 : entry.menu) === null || _a === void 0 ? void 0 : _a.items) === null || _b === void 0 ? void 0 : _b.find((i) => i.id === id);
            if (mi && typeof mi.click === 'function') {
                const bw = electron_1.BrowserWindow.fromId(windowId);
                mi.click(undefined, bw, undefined);
            }
        }
        catch { /* ignore */ }
    });
};
//# sourceMappingURL=ipc.js.map