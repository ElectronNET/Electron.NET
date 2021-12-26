"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('desktop-capturer-get-sources', (options) => {
        electron_1.desktopCapturer.getSources(options).then(sources => {
            electronSocket.emit('desktop-capturer-get-sources-result', sources);
        });
    });
};
//# sourceMappingURL=desktopCapturer.js.map