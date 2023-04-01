"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('clipboard-readText', (type) => {
        const text = electron_1.clipboard.readText(type);
        electronSocket.emit('clipboard-readText-Completed', text);
    });
    socket.on('clipboard-writeText', (text, type) => {
        electron_1.clipboard.writeText(text, type);
    });
    socket.on('clipboard-readHTML', (type) => {
        const content = electron_1.clipboard.readHTML(type);
        electronSocket.emit('clipboard-readHTML-Completed', content);
    });
    socket.on('clipboard-writeHTML', (markup, type) => {
        electron_1.clipboard.writeHTML(markup, type);
    });
    socket.on('clipboard-readRTF', (type) => {
        const content = electron_1.clipboard.readRTF(type);
        electronSocket.emit('clipboard-readRTF-Completed', content);
    });
    socket.on('clipboard-writeRTF', (text, type) => {
        electron_1.clipboard.writeHTML(text, type);
    });
    socket.on('clipboard-readBookmark', () => {
        const bookmark = electron_1.clipboard.readBookmark();
        electronSocket.emit('clipboard-readBookmark-Completed', bookmark);
    });
    socket.on('clipboard-writeBookmark', (title, url, type) => {
        electron_1.clipboard.writeBookmark(title, url, type);
    });
    socket.on('clipboard-readFindText', () => {
        const content = electron_1.clipboard.readFindText();
        electronSocket.emit('clipboard-readFindText-Completed', content);
    });
    socket.on('clipboard-writeFindText', (text) => {
        electron_1.clipboard.writeFindText(text);
    });
    socket.on('clipboard-clear', (type) => {
        electron_1.clipboard.clear(type);
    });
    socket.on('clipboard-availableFormats', (type) => {
        const formats = electron_1.clipboard.availableFormats(type);
        electronSocket.emit('clipboard-availableFormats-Completed', formats);
    });
    socket.on('clipboard-write', (data, type) => {
        electron_1.clipboard.write(data, type);
    });
    socket.on('clipboard-readImage', (type) => {
        const image = electron_1.clipboard.readImage(type);
        electronSocket.emit('clipboard-readImage-Completed', { 1: image.toPNG().toString('base64') });
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