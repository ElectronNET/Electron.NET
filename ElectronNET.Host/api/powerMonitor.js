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
    socket.on('register-pm-suspend', () => {
        electron_1.powerMonitor.on('suspend', () => {
            electronSocket.emit('pm-suspend');
        });
    });
    socket.on('register-pm-resume', () => {
        electron_1.powerMonitor.on('resume', () => {
            electronSocket.emit('pm-resume');
        });
    });
    socket.on('register-pm-on-ac', () => {
        electron_1.powerMonitor.on('on-ac', () => {
            electronSocket.emit('pm-on-ac');
        });
    });
    socket.on('register-pm-on-battery', () => {
        electron_1.powerMonitor.on('on-battery', () => {
            electronSocket.emit('pm-on-battery');
        });
    });
    socket.on('register-pm-shutdown', () => {
        electron_1.powerMonitor.on('shutdown', () => {
            electronSocket.emit('pm-shutdown');
        });
    });
};
//# sourceMappingURL=powerMonitor.js.map