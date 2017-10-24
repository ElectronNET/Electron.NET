"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('registerIpcMainChannel', function (channel) {
        electron_1.ipcMain.on(channel, function (event, args) {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerSyncIpcMainChannel', function (channel) {
        electron_1.ipcMain.on(channel, function (event, args) {
            var x = socket;
            x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', function (result) {
                event.returnValue = result;
            });
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', function (channel) {
        electron_1.ipcMain.once(channel, function (event, args) {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('removeAllListenersIpcMainChannel', function (channel) {
        electron_1.ipcMain.removeAllListeners(channel);
    });
    socket.on('sendToIpcRenderer', function (browserWindow, channel) {
        var data = [];
        for (var _i = 2; _i < arguments.length; _i++) {
            data[_i - 2] = arguments[_i];
        }
        var window = electron_1.BrowserWindow.fromId(browserWindow.id);
        if (window) {
            (_a = window.webContents).send.apply(_a, [channel].concat(data));
        }
        var _a;
    });
};
//# sourceMappingURL=ipc.js.map