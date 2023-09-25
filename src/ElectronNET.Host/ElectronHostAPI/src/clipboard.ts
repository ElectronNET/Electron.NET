import { clipboard, nativeImage } from 'electron';
import { Socket } from 'node:net';

let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;
    socket.on('clipboard-readText', (type) => {
        const text = clipboard.readText(type);
        electronSocket.emit('clipboard-readText-Completed', text);
    });

    socket.on('clipboard-writeText', (text, type) => {
        clipboard.writeText(text, type);
    });

    socket.on('clipboard-readHTML', (type) => {
        const content = clipboard.readHTML(type);
        electronSocket.emit('clipboard-readHTML-Completed', content);
    });

    socket.on('clipboard-writeHTML', (markup, type) => {
        clipboard.writeHTML(markup, type);
    });

    socket.on('clipboard-readRTF', (type) => {
        const content = clipboard.readRTF(type);
        electronSocket.emit('clipboard-readRTF-Completed', content);
    });

    socket.on('clipboard-writeRTF', (text, type) => {
        clipboard.writeHTML(text, type);
    });

    socket.on('clipboard-readBookmark', () => {
        const bookmark = clipboard.readBookmark();
        electronSocket.emit('clipboard-readBookmark-Completed', bookmark);
    });

    socket.on('clipboard-writeBookmark', (title, url, type) => {
        clipboard.writeBookmark(title, url, type);
    });

    socket.on('clipboard-readFindText', () => {
        const content = clipboard.readFindText();
        electronSocket.emit('clipboard-readFindText-Completed', content);
    });

    socket.on('clipboard-writeFindText', (text) => {
        clipboard.writeFindText(text);
    });

    socket.on('clipboard-clear', (type) => {
        clipboard.clear(type);
    });

    socket.on('clipboard-availableFormats', (type) => {
        const formats = clipboard.availableFormats(type);
        electronSocket.emit('clipboard-availableFormats-Completed', formats);
    });

    socket.on('clipboard-write', (data, type) => {
        clipboard.write(data, type);
    });

    socket.on('clipboard-readImage', (type) => {
        const image = clipboard.readImage(type);
        electronSocket.emit('clipboard-readImage-Completed', { 1: image.toPNG().toString('base64') });
    });

    socket.on('clipboard-writeImage', (data, type) => {
        const dataContent = JSON.parse(data);
        const image = nativeImage.createEmpty();

        // tslint:disable-next-line: forin
        for (const key in dataContent) {
            const scaleFactor = key;
            const bytes = data[key];
            const buffer = Buffer.from(bytes, 'base64');
            image.addRepresentation({ scaleFactor: +scaleFactor, buffer: buffer });
        }

        clipboard.writeImage(image, type);
    });
};
