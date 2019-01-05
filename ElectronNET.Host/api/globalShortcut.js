"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('globalShortcut-register', (accelerator) => {
        electron_1.globalShortcut.register(accelerator, () => {
            socket.emit('globalShortcut-pressed', accelerator);
        });
    });
    socket.on('globalShortcut-isRegistered', (accelerator) => {
        const isRegistered = electron_1.globalShortcut.isRegistered(accelerator);
        socket.emit('globalShortcut-isRegisteredCompleted', isRegistered);
    });
    socket.on('globalShortcut-unregister', (accelerator) => {
        electron_1.globalShortcut.unregister(accelerator);
    });
    socket.on('globalShortcut-unregisterAll', () => {
        try {
            electron_1.globalShortcut.unregisterAll();
        }
        catch (error) { }
    });
};
//# sourceMappingURL=globalShortcut.js.map