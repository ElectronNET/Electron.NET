"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('shell-showItemInFolder', (guid, fullPath) => {
        electron_1.shell.showItemInFolder(fullPath);
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('shell-openPath', async (guid, path) => {
        const errorMessage = await electron_1.shell.openPath(path);
        socket.invoke('SendClientResponseString', guid, errorMessage);
    });
    socket.on('shell-openExternal', async (guid, url) => {
        let result = '';
        await electron_1.shell.openExternal(url).catch((e) => {
            result = e.message;
        });
        socket.invoke('SendClientResponseString', guid, result);
    });
    socket.on('shell-openExternal-options', async (guid, url, options) => {
        let result = '';
        await electron_1.shell.openExternal(url, options).catch(e => {
            result = e.message;
        });
        socket.invoke('SendClientResponseString', guid, result);
    });
    socket.on('shell-trashItem', async (guid, fullPath, deleteOnFail) => {
        let success = false;
        try {
            await electron_1.shell.trashItem(fullPath);
            success = true;
        }
        catch (error) {
            success = false;
        }
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('shell-beep', () => {
        electron_1.shell.beep();
    });
    socket.on('shell-writeShortcutLink', (guid, shortcutPath, operation, options) => {
        const success = electron_1.shell.writeShortcutLink(shortcutPath, operation, options);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('shell-readShortcutLink', (guid, shortcutPath) => {
        const shortcutDetails = electron_1.shell.readShortcutLink(shortcutPath);
        socket.invoke('SendClientResponseJObject', guid, shortcutDetails);
    });
};
//# sourceMappingURL=shell.js.map