"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('clipboard-readText', (type) => {
        const text = electron_1.clipboard.readText(type);
        socket.emit('clipboard-readText-Completed', text);
    });
    socket.on('clipboard-writeText', (text, type) => {
        electron_1.clipboard.writeText(text, type);
    });
    socket.on('clipboard-readHTML', (type) => {
        const content = electron_1.clipboard.readHTML(type);
        socket.emit('clipboard-readHTML-Completed', content);
    });
    socket.on('clipboard-writeHTML', (markup, type) => {
        electron_1.clipboard.writeHTML(markup, type);
    });
    socket.on('clipboard-readRTF', (type) => {
        const content = electron_1.clipboard.readRTF(type);
        socket.emit('clipboard-readRTF-Completed', content);
    });
    socket.on('clipboard-writeRTF', (text, type) => {
        electron_1.clipboard.writeHTML(text, type);
    });
    socket.on('clipboard-readBookmark', () => {
        const bookmark = electron_1.clipboard.readBookmark();
        socket.emit('clipboard-readBookmark-Completed', bookmark);
    });
    socket.on('clipboard-writeBookmark', (title, url, type) => {
        electron_1.clipboard.writeBookmark(title, url, type);
    });
    socket.on('clipboard-readFindText', () => {
        const content = electron_1.clipboard.readFindText();
        socket.emit('clipboard-readFindText-Completed', content);
    });
    socket.on('clipboard-writeFindText', (text) => {
        electron_1.clipboard.writeFindText(text);
    });
    socket.on('clipboard-clear', (type) => {
        electron_1.clipboard.clear(type);
    });
    socket.on('clipboard-availableFormats', (type) => {
        const formats = electron_1.clipboard.availableFormats(type);
        socket.emit('clipboard-availableFormats-Completed', formats);
    });
    socket.on('clipboard-write', (data, type) => {
        electron_1.clipboard.write(data, type);
    });
};
//# sourceMappingURL=clipboard.js.map