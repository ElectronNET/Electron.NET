"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('shell-showItemInFolder', (fullPath) => {
        electron_1.shell.showItemInFolder(fullPath);
        electronSocket.emit('shell-showItemInFolderCompleted');
    });
    socket.on('shell-openPath', async (path) => {
        const errorMessage = await electron_1.shell.openPath(path);
        electronSocket.emit('shell-openPathCompleted', errorMessage);
    });
    socket.on('shell-openExternal', async (url, options) => {
        let result = '';
        if (options) {
            await electron_1.shell.openExternal(url, options).catch(e => {
                result = e.message;
            });
        }
        else {
            await electron_1.shell.openExternal(url).catch((e) => {
                result = e.message;
            });
        }
        electronSocket.emit('shell-openExternalCompleted', result);
    });
    socket.on('shell-moveItemToTrash', (fullPath, deleteOnFail) => {
        const success = electron_1.shell.moveItemToTrash(fullPath, deleteOnFail);
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