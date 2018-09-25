"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('clipboard-readText', function (type) {
        var text = electron_1.clipboard.readText(type);
        socket.emit('clipboard-readText-Completed', text);
    });
    socket.on('clipboard-writeText', function (text, type) {
        electron_1.clipboard.writeText(text, type);
    });
    socket.on('clipboard-readHTML', function (type) {
        var content = electron_1.clipboard.readHTML(type);
        socket.emit('clipboard-readHTML-Completed', content);
    });
    socket.on('clipboard-writeHTML', function (markup, type) {
        electron_1.clipboard.writeHTML(markup, type);
    });
    socket.on('clipboard-readRTF', function (type) {
        var content = electron_1.clipboard.readRTF(type);
        socket.emit('clipboard-readRTF-Completed', content);
    });
    socket.on('clipboard-writeRTF', function (text, type) {
        electron_1.clipboard.writeHTML(text, type);
    });
    socket.on('clipboard-readBookmark', function () {
        var bookmark = electron_1.clipboard.readBookmark();
        socket.emit('clipboard-readBookmark-Completed', bookmark);
    });
    socket.on('clipboard-writeBookmark', function (title, url, type) {
        electron_1.clipboard.writeBookmark(title, url, type);
    });
    socket.on('clipboard-readFindText', function () {
        var content = electron_1.clipboard.readFindText();
        socket.emit('clipboard-readFindText-Completed', content);
    });
    socket.on('clipboard-writeFindText', function (text) {
        electron_1.clipboard.writeFindText(text);
    });
    socket.on('clipboard-clear', function (type) {
        electron_1.clipboard.clear(type);
    });
    socket.on('clipboard-availableFormats', function (type) {
        var formats = electron_1.clipboard.availableFormats(type);
        socket.emit('clipboard-availableFormats-Completed', formats);
    });
    socket.on('clipboard-write', function (data, type) {
        electron_1.clipboard.write(data, type);
    });
};
//# sourceMappingURL=clipboard.js.map