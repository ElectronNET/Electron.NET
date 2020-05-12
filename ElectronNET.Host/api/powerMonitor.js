"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('register-pm-lock-screen', () => {
        electron_1.powerMonitor.on('lock-screen', () => {
            electronSocket.emit('pm-lock-screen');
        });
    });
    socket.on('register-pm-unlock-screen', () => {
        electron_1.powerMonitor.on('unlock-screen', () => {
            electronSocket.emit('pm-unlock-screen');
        });
    });
};
//# sourceMappingURL=powerMonitor.js.map