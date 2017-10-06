var ipcMain = require('electron').ipcMain;
module.exports = function (socket, window) {
    socket.on('registerIpcMainChannel', function (channel) {
        ipcMain.on(channel, function (event, args) {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('registerOnceIpcMainChannel', function (channel) {
        ipcMain.once(channel, function (event, args) {
            socket.emit(channel, [event.preventDefault(), args]);
        });
    });
    socket.on('removeAllListenersIpcMainChannel', function (channel) {
        ipcMain.removeAllListeners(channel);
    });
    socket.on('sendToIpcRenderer', function (channel) {
        var data = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            data[_i - 1] = arguments[_i];
        }
        if (window) {
            window.webContents.send(channel, data);
        }
    });
};
//# sourceMappingURL=ipc.js.map