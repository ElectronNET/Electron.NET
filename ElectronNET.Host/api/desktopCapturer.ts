import { Socket } from 'net';
import { desktopCapturer } from 'electron';

let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;
    socket.on('desktop-capturer-get-sources', (options) => {
        desktopCapturer.getSources(options).then(sources => {
            electronSocket.emit('desktop-capturer-get-sources-result', sources);
        });
    });
};
