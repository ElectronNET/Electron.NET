"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('shell-showItemInFolder', (fullPath) => {
        const success = electron_1.shell.showItemInFolder(fullPath);
        electronSocket.emit('shell-showItemInFolderCompleted', success);
    });
    socket.on('shell-openItem', (fullPath) => {
        const success = electron_1.shell.openItem(fullPath);
        electronSocket.emit('shell-openItemCompleted', success);
    });
    socket.on('shell-openExternal', (url, options) => {
        let success = true;
        if (options) {
            electron_1.shell.openExternal(url, options).catch((error) => {
                success = false;
                electronSocket.emit('shell-openExternalCallback', [url, error]);
            });
        }
        else {
            electron_1.shell.openExternal(url).catch((error) => {
                success = false;
                electronSocket.emit('shell-openExternalCallback', [url, error]);
            });
        }
        electronSocket.emit('shell-openExternalCompleted', success);
    });
    socket.on('shell-moveItemToTrash', (fullPath) => {
        const success = electron_1.shell.moveItemToTrash(fullPath);
        electronSocket.emit('shell-moveItemToTrashCompleted', success);
    });
    socket.on('shell-beep', () => {
        electron_1.shell.beep();
    });
    socket.on('shell-writeShortcutLink', (shortcutPath, operation, options) => {
        const success = electron_1.shell.writeShortcutLink(shortcutPath, operation, options);
        electronSocket.emit('shell-writeShortcutLinkCompleted', success);
    });
    socket.on('shell-readShortcutLink', (shortcutPath) => {
        const shortcutDetails = electron_1.shell.readShortcutLink(shortcutPath);
        electronSocket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
};
//# sourceMappingURL=shell.js.map