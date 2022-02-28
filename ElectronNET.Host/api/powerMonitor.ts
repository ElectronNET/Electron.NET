import { powerMonitor } from 'electron';

export = (socket: SignalR.Hub.Proxy) => {
    socket.on('register-pm-lock-screen', () => {
        powerMonitor.on('lock-screen', () => {
            socket.invoke('TriggerOnLockScreen');
        });
    });
    socket.on('register-pm-unlock-screen', () => {
        powerMonitor.on('unlock-screen', () => {
            socket.invoke('PowerMonitorOnUnLockScreen');
        });
    });
    socket.on('register-pm-suspend', () => {
        powerMonitor.on('suspend', () => {
            socket.invoke('PowerMonitorOnSuspend');
        });
    });
    socket.on('register-pm-resume', () => {
        powerMonitor.on('resume', () => {
            socket.invoke('PowerMonitorOnResume');
        });
    });
    socket.on('register-pm-on-ac', () => {
        powerMonitor.on('on-ac', () => {
            socket.invoke('PowerMonitorOnAC');
        });
    });
    socket.on('register-pm-on-battery', () => {
        powerMonitor.on('on-battery', () => {
            socket.invoke('PowerMonitorOnBattery');
        });
    });
    socket.on('register-pm-shutdown', () => {
        powerMonitor.on('shutdown', () => {
            socket.invoke('PowerMonitorOnShutdown');
        });
    });
};
