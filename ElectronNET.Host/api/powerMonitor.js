"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('register-pm-lock-screen', () => {
        electron_1.powerMonitor.on('lock-screen', () => {
            socket.invoke('TriggerOnLockScreen');
        });
    });
    socket.on('register-pm-unlock-screen', () => {
        electron_1.powerMonitor.on('unlock-screen', () => {
            socket.invoke('PowerMonitorOnUnLockScreen');
        });
    });
    socket.on('register-pm-suspend', () => {
        electron_1.powerMonitor.on('suspend', () => {
            socket.invoke('PowerMonitorOnSuspend');
        });
    });
    socket.on('register-pm-resume', () => {
        electron_1.powerMonitor.on('resume', () => {
            socket.invoke('PowerMonitorOnResume');
        });
    });
    socket.on('register-pm-on-ac', () => {
        electron_1.powerMonitor.on('on-ac', () => {
            socket.invoke('PowerMonitorOnAC');
        });
    });
    socket.on('register-pm-on-battery', () => {
        electron_1.powerMonitor.on('on-battery', () => {
            socket.invoke('PowerMonitorOnBattery');
        });
    });
    socket.on('register-pm-shutdown', () => {
        electron_1.powerMonitor.on('shutdown', () => {
            socket.invoke('PowerMonitorOnShutdown');
        });
    });
};
//# sourceMappingURL=powerMonitor.js.map