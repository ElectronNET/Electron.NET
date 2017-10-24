import { clipboard } from "electron";

module.exports = (socket: SocketIO.Server) => {

    socket.on('clipboard-readText', (type) => {
        const text = clipboard.readText(type);
        socket.emit('clipboard-readText-Completed', text);
    });

    socket.on('clipboard-writeText', (text, type) => {
        clipboard.writeText(text, type);
    });

    socket.on('clipboard-readHTML', (type) => {
        const content = clipboard.readHTML(type);
        socket.emit('clipboard-readHTML-Completed', content);
    });

    socket.on('clipboard-writeHTML', (markup, type) => {
        clipboard.writeHTML(markup, type);
    });

    socket.on('clipboard-readRTF', (type) => {
        const content = clipboard.readRTF(type);
        socket.emit('clipboard-readRTF-Completed', content);
    });

    socket.on('clipboard-writeRTF', (text, type) => {
        clipboard.writeHTML(text, type);
    });

    socket.on('clipboard-readBookmark', () => {
        const bookmark = clipboard.readBookmark();
        socket.emit('clipboard-readBookmark-Completed', bookmark);
    });

    socket.on('clipboard-writeBookmark', (title, url, type) => {
        clipboard.writeBookmark(title, url, type);
    });

    socket.on('clipboard-readFindText', () => {
        const content = clipboard.readFindText();
        socket.emit('clipboard-readFindText-Completed', content);
    });

    socket.on('clipboard-writeFindText', (text) => {
        clipboard.writeFindText(text);
    });

    socket.on('clipboard-clear', (type) => {
        clipboard.clear(type);
    });

    socket.on('clipboard-availableFormats', (type) => {
        const formats = clipboard.availableFormats(type);
        socket.emit('clipboard-availableFormats-Completed', formats);
    });

    socket.on('clipboard-write', (data, type) => {
        clipboard.write(data, type);
    });
}