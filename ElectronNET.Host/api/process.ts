import { Socket } from 'net';
let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;

    socket.on('process-execPath', () => {
        const value = process.execPath;
        electronSocket.emit('process-execPathCompleted', value);
    });
};
