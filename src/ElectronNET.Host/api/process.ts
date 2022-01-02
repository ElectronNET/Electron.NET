import { Socket } from 'net';
let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;

    socket.on('process-execPath', () => {
        const value = process.execPath;
        electronSocket.emit('process-execPathCompleted', value);
    });

    socket.on('process-argv', () => {
        const value = process.argv;
        electronSocket.emit('process-argvCompleted', value);
    });

    socket.on('process-type', () => {
        const value = process.type;
        electronSocket.emit('process-typeCompleted', value);
    });
};
