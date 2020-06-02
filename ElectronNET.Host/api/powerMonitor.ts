import { powerMonitor } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('register-pm-lock-screen', () => {
        powerMonitor.on('lock-screen', () => {
            electronSocket.emit('pm-lock-screen');
        });
    });
    socket.on('register-pm-unlock-screen', () => {
        powerMonitor.on('unlock-screen', () => {
            electronSocket.emit('pm-unlock-screen');
        });
    });
    socket.on('register-pm-suspend', () => {
        powerMonitor.on('suspend', () => {
            electronSocket.emit('pm-suspend');
        });
    });
    socket.on('register-pm-resume', () => {
        powerMonitor.on('resume', () => {
            electronSocket.emit('pm-resume');
        });
    });
    socket.on('register-pm-on-ac', () => {
        powerMonitor.on('on-ac', () => {
            electronSocket.emit('pm-on-ac');
        });
    });
    socket.on('register-pm-on-battery', () => {
        powerMonitor.on('on-battery', () => {
            electronSocket.emit('pm-on-battery');
        });
    });
    socket.on('register-pm-shutdown', () => {
        powerMonitor.on('shutdown', () => {
            electronSocket.emit('pm-shutdown');
        });
    });
};
