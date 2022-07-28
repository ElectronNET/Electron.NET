import { Socket } from 'net';
import { desktopCapturer } from 'electron';

let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;
    socket.on('desktop-capturer-get-sources', (options) => {
        desktopCapturer.getSources(options).then(sources => {

            const result = sources.map(src => {
                return {
                    appIcon: src.appIcon,
                    name: src.name,
                    id: src.id,
                    display_id: src.display_id,
                    thumbnail: { 1: src.thumbnail.toPNG().toString('base64') }
                };
            });

            electronSocket.emit('desktop-capturer-get-sources-result', result);
        });
    });


};;
