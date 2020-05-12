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
};
