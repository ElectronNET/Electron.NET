import { HubConnection  } from "@microsoft/signalr";
import { clipboard, nativeImage } from 'electron';

export = (socket: HubConnection) => {

    socket.on('clipboard-readText', (guid, type) => {
        const text = clipboard.readText(type);
        socket.invoke('SendClientResponseString', guid, text);
    });

    socket.on('clipboard-writeText', (text, type) => {
        clipboard.writeText(text, type);
    });

    socket.on('clipboard-readHTML', (guid, type) => {
        const content = clipboard.readHTML(type);
        socket.invoke('SendClientResponseString', guid, content);
    });

    socket.on('clipboard-writeHTML', (markup, type) => {
        clipboard.writeHTML(markup, type);
    });

    socket.on('clipboard-readRTF', (guid, type) => {
        const content = clipboard.readRTF(type);
        socket.invoke('SendClientResponseString', guid, content);
    });

    socket.on('clipboard-writeRTF', (text, type) => {
        clipboard.writeHTML(text, type);
    });

    socket.on('clipboard-readBookmark', (guid) => {
        const bookmark = clipboard.readBookmark();
        socket.invoke('SendClientResponseJObject', guid, bookmark);
    });

    socket.on('clipboard-writeBookmark', (title, url, type) => {
        clipboard.writeBookmark(title, url, type);
    });

    socket.on('clipboard-readFindText', (guid) => {
        const content = clipboard.readFindText();
        socket.invoke('SendClientResponseString', guid, content);
    });

    socket.on('clipboard-writeFindText', (text) => {
        clipboard.writeFindText(text);
    });

    socket.on('clipboard-clear', (type) => {
        clipboard.clear(type);
    });

    socket.on('clipboard-availableFormats', (guid, type) => {
        const formats = clipboard.availableFormats(type);
        socket.invoke('SendClientResponseJArray', guid, formats);
    });

    socket.on('clipboard-write', (data, type) => {
        clipboard.write(data, type);
    });

    socket.on('clipboard-readImage', (guid, type) => {
        const image = clipboard.readImage(type);
        socket.invoke('SendClientResponseJObject', guid, { 1: image.toPNG().toString('base64') });
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
