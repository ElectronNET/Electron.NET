"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('shell-showItemInFolder', (fullPath) => {
        const success = electron_1.shell.showItemInFolder(fullPath);
        socket.emit('shell-showItemInFolderCompleted', success);
    });
    socket.on('shell-openItem', (fullPath) => {
        const success = electron_1.shell.openItem(fullPath);
        socket.emit('shell-openItemCompleted', success);
    });
    socket.on('shell-openExternal', (url, options, callback) => {
        let success = false;
        if (options && callback) {
            success = electron_1.shell.openExternal(url, options, (error) => {
                socket.emit('shell-openExternalCallback', [url, error]);
            });
        }
        else if (options) {
            success = electron_1.shell.openExternal(url, options);
        }
        else {
            success = electron_1.shell.openExternal(url);
        }
        socket.emit('shell-openExternalCompleted', success);
    });
    socket.on('shell-moveItemToTrash', (fullPath) => {
        const success = electron_1.shell.moveItemToTrash(fullPath);
        socket.emit('shell-moveItemToTrashCompleted', success);
    });
    socket.on('shell-beep', () => {
        electron_1.shell.beep();
    });
    socket.on('shell-writeShortcutLink', (shortcutPath, operation, options) => {
        const success = electron_1.shell.writeShortcutLink(shortcutPath, operation, options);
        socket.emit('shell-writeShortcutLinkCompleted', success);
    });
    socket.on('shell-readShortcutLink', (shortcutPath) => {
        const shortcutDetails = electron_1.shell.readShortcutLink(shortcutPath);
        socket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
};
//# sourceMappingURL=shell.js.map