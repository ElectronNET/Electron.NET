"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('registerIpcMainChannel', function (channel) { // 监听主程序注册
        electron_1.ipcMain.removeAllListeners(channel); // yf add 防止重复监听
        electron_1.ipcMain.on(channel, function (event, args) { // 监听前端的推送
            global.elesocket.emit(channel, [event.preventDefault(), args]); // 发送到后端
        });
    });
    socket.on('registerSyncIpcMainChannel', function (channel) {
        electron_1.ipcMain.on(channel, function (event, args) {
            var x = global.elesocket;
            x.removeAllListeners(channel + 'Sync');
            global.elesocket.on(channel + 'Sync', function (result) {
                event.returnValue = result;
            });
            global.elesocket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', function (channel) {
        electron_1.ipcMain.once(channel, function (event, args) {
            global.elesocket.emit(channel, [event.preventDefault(), args]);
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