"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on("register-powerMonitor-lock-screen", () => {
        electron_1.powerMonitor.on("lock-screen", () => {
            electronSocket.emit("powerMonitor-lock-screen");
        });
    });
    socket.on("register-powerMonitor-unlock-screen", () => {
        electron_1.powerMonitor.on("unlock-screen", () => {
            electronSocket.emit("powerMonitor-unlock-screen");
        });
    });
    socket.on("register-powerMonitor-suspend", () => {
        electron_1.powerMonitor.on("suspend", () => {
            electronSocket.emit("powerMonitor-suspend");
        });
    });
    socket.on("register-powerMonitor-resume", () => {
        electron_1.powerMonitor.on("resume", () => {
            electronSocket.emit("powerMonitor-resume");
        });
    });
    socket.on("register-powerMonitor-ac", () => {
        electron_1.powerMonitor.on("on-ac", () => {
            electronSocket.emit("powerMonitor-ac");
        });
    });
    socket.on("register-powerMonitor-battery", () => {
        electron_1.powerMonitor.on("on-battery", () => {
            electronSocket.emit("powerMonitor-battery");
        });
    });
    socket.on("register-powerMonitor-shutdown", () => {
        electron_1.powerMonitor.on("shutdown", () => {
            electronSocket.emit("powerMonitor-shutdown");
        });
    });
};
//# sourceMappingURL=powerMonitor.js.map