"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('nativeTheme-shouldUseDarkColors', () => {
        const shouldUseDarkColors = electron_1.nativeTheme.shouldUseDarkColors;
        electronSocket.emit('nativeTheme-shouldUseDarkColors-completed', shouldUseDarkColors);
    });
};
//# sourceMappingURL=nativeTheme.js.map