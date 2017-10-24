import { shell } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('shell-showItemInFolder', (fullPath) => {
        const success = shell.showItemInFolder(fullPath);

        socket.emit('shell-showItemInFolderCompleted', success);
    });

    socket.on('shell-openItem', (fullPath) => {
        const success = shell.openItem(fullPath);

        socket.emit('shell-openItemCompleted', success);
    });

    socket.on('shell-openExternal', (url, options, callback) => {
        let success = false;

        if (options && callback) {
            success = shell.openExternal(url, options, (error) => {
                socket.emit('shell-openExternalCallback', [url, error]);
            });
        } else if (options) {
            success = shell.openExternal(url, options);
        } else {
            success = shell.openExternal(url);
        }

        socket.emit('shell-openExternalCompleted', success);
    });

    socket.on('shell-moveItemToTrash', (fullPath) => {
        const success = shell.moveItemToTrash(fullPath);

        socket.emit('shell-moveItemToTrashCompleted', success);
    });

    socket.on('shell-beep', () => {
        shell.beep();
    });

    socket.on('shell-writeShortcutLink', (shortcutPath, operation, options) => {
        const success = shell.writeShortcutLink(shortcutPath, operation, options);

        socket.emit('shell-writeShortcutLinkCompleted', success);
    });

    socket.on('shell-readShortcutLink', (shortcutPath) => {
        const shortcutDetails = shell.readShortcutLink(shortcutPath);

        socket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
}