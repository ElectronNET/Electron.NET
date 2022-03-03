"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('clipboard-readText', (guid, type) => {
        const text = electron_1.clipboard.readText(type);
        socket.invoke('SendClientResponseString', guid, text);
    });
    socket.on('clipboard-writeText', (text, type) => {
        electron_1.clipboard.writeText(text, type);
    });
    socket.on('clipboard-readHTML', (guid, type) => {
        const content = electron_1.clipboard.readHTML(type);
        socket.invoke('SendClientResponseString', guid, content);
    });
    socket.on('clipboard-writeHTML', (markup, type) => {
        electron_1.clipboard.writeHTML(markup, type);
    });
    socket.on('clipboard-readRTF', (guid, type) => {
        const content = electron_1.clipboard.readRTF(type);
        socket.invoke('SendClientResponseString', guid, content);
    });
    socket.on('clipboard-writeRTF', (text, type) => {
        electron_1.clipboard.writeHTML(text, type);
    });
    socket.on('clipboard-readBookmark', (guid) => {
        const bookmark = electron_1.clipboard.readBookmark();
        socket.invoke('SendClientResponseJObject', guid, bookmark);
    });
    socket.on('clipboard-writeBookmark', (title, url, type) => {
        electron_1.clipboard.writeBookmark(title, url, type);
    });
    socket.on('clipboard-readFindText', (guid) => {
        const content = electron_1.clipboard.readFindText();
        socket.invoke('SendClientResponseString', guid, content);
    });
    socket.on('clipboard-writeFindText', (text) => {
        electron_1.clipboard.writeFindText(text);
    });
    socket.on('clipboard-clear', (type) => {
        electron_1.clipboard.clear(type);
    });
    socket.on('clipboard-availableFormats', (guid, type) => {
        const formats = electron_1.clipboard.availableFormats(type);
        socket.invoke('SendClientResponseJArray', guid, formats);
    });
    socket.on('clipboard-write', (data, type) => {
        electron_1.clipboard.write(data, type);
    });
    socket.on('clipboard-readImage', (guid, type) => {
        const image = electron_1.clipboard.readImage(type);
        socket.invoke('SendClientResponseJObject', guid, { 1: image.toPNG().toString('base64') });
    });
    socket.on('clipboard-writeImage', (data, type) => {
        const dataContent = JSON.parse(data);
        const image = electron_1.nativeImage.createEmpty();
        // tslint:disable-next-line: forin
        for (const key in dataContent) {
            const scaleFactor = key;
            const bytes = data[key];
            const buffer = Buffer.from(bytes, 'base64');
            image.addRepresentation({ scaleFactor: +scaleFactor, buffer: buffer });
        }
        electron_1.clipboard.writeImage(image, type);
    });
};
//# sourceMappingURL=clipboard.js.map