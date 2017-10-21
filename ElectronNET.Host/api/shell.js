"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('shell-showItemInFolder', function (fullPath) {
        var success = electron_1.shell.showItemInFolder(fullPath);
        socket.emit('shell-showItemInFolderCompleted', success);
    });
    socket.on('shell-openItem', function (fullPath) {
        var success = electron_1.shell.openItem(fullPath);
        socket.emit('shell-openItemCompleted', success);
    });
    socket.on('shell-openExternal', function (url, options, callback) {
        var success = false;
        if (options && callback) {
            success = electron_1.shell.openExternal(url, options, function (error) {
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
    socket.on('shell-moveItemToTrash', function (fullPath) {
        var success = electron_1.shell.moveItemToTrash(fullPath);
        socket.emit('shell-moveItemToTrashCompleted', success);
    });
    socket.on('shell-beep', function () {
        electron_1.shell.beep();
    });
    socket.on('shell-writeShortcutLink', function (shortcutPath, operation, options) {
        var success = electron_1.shell.writeShortcutLink(shortcutPath, operation, options);
        socket.emit('shell-writeShortcutLinkCompleted', success);
    });
    socket.on('shell-readShortcutLink', function (shortcutPath) {
        var shortcutDetails = electron_1.shell.readShortcutLink(shortcutPath);
        socket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
};
//# sourceMappingURL=shell.js.map