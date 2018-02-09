"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('shell-showItemInFolder', function (fullPath) {
        var success = electron_1.shell.showItemInFolder(fullPath);
        global.elesocket.emit('shell-showItemInFolderCompleted', success);
    });
    socket.on('shell-openItem', function (fullPath) {
        var success = electron_1.shell.openItem(fullPath);
        global.elesocket.emit('shell-openItemCompleted', success);
    });
    socket.on('shell-openExternal', function (url, options, callback) {
        var success = false;
        if (options && callback) {
            success = electron_1.shell.openExternal(url, options, function (error) {
                global.elesocket.emit('shell-openExternalCallback', [url, error]);
            });
        }
        else if (options) {
            success = electron_1.shell.openExternal(url, options);
        }
        else {
            success = electron_1.shell.openExternal(url);
        }
        global.elesocket.emit('shell-openExternalCompleted', success);
    });
    socket.on('shell-moveItemToTrash', function (fullPath) {
        var success = electron_1.shell.moveItemToTrash(fullPath);
        global.elesocket.emit('shell-moveItemToTrashCompleted', success);
    });
    socket.on('shell-beep', function () {
        electron_1.shell.beep();
    });
    socket.on('shell-writeShortcutLink', function (shortcutPath, operation, options) {
        var success = electron_1.shell.writeShortcutLink(shortcutPath, operation, options);
        global.elesocket.emit('shell-writeShortcutLinkCompleted', success);
    });
    socket.on('shell-readShortcutLink', function (shortcutPath) {
        var shortcutDetails = electron_1.shell.readShortcutLink(shortcutPath);
        global.elesocket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
};
//# sourceMappingURL=shell.js.map