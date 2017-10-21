"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('globalShortcut-register', function (accelerator) {
        electron_1.globalShortcut.register(accelerator, function () {
            socket.emit('globalShortcut-pressed', accelerator);
        });
    });
    socket.on('globalShortcut-isRegistered', function (accelerator) {
        var isRegistered = electron_1.globalShortcut.isRegistered(accelerator);
        socket.emit('globalShortcut-isRegisteredCompleted', isRegistered);
    });
    socket.on('globalShortcut-unregister', function (accelerator) {
        electron_1.globalShortcut.unregister(accelerator);
    });
    socket.on('globalShortcut-unregisterAll', function () {
        try {
            electron_1.globalShortcut.unregisterAll();
        }
        catch (error) { }
    });
};
//# sourceMappingURL=globalShortcut.js.map