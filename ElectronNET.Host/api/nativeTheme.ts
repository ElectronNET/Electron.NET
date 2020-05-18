import { nativeTheme } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;

    socket.on('nativeTheme-shouldUseDarkColors', () => {
        const shouldUseDarkColors = nativeTheme.shouldUseDarkColors;

        electronSocket.emit('nativeTheme-shouldUseDarkColors-completed', shouldUseDarkColors);
    });
};
