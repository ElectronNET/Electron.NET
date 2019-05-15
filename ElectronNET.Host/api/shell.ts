import { shell } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('shell-showItemInFolder', (fullPath) => {
        const success = shell.showItemInFolder(fullPath);

        electronSocket.emit('shell-showItemInFolderCompleted', success);
    });

    socket.on('shell-openItem', (fullPath) => {
        const success = shell.openItem(fullPath);

        electronSocket.emit('shell-openItemCompleted', success);
    });

    socket.on('shell-openExternal', (url, options, callback) => {
        let success = false;

        if (options && callback) {
            success = shell.openExternal(url, options, (error) => {
                electronSocket.emit('shell-openExternalCallback', [url, error]);
            });
        } else if (options) {
            success = shell.openExternal(url, options);
        } else {
            success = shell.openExternal(url);
        }

        electronSocket.emit('shell-openExternalCompleted', success);
    });

    socket.on('shell-moveItemToTrash', (fullPath) => {
        const success = shell.moveItemToTrash(fullPath);

        electronSocket.emit('shell-moveItemToTrashCompleted', success);
    });

    socket.on('shell-beep', () => {
        shell.beep();
    });

    socket.on('shell-writeShortcutLink', (shortcutPath, operation, options) => {
        const success = shell.writeShortcutLink(shortcutPath, operation, options);

        electronSocket.emit('shell-writeShortcutLinkCompleted', success);
    });

    socket.on('shell-readShortcutLink', (shortcutPath) => {
        const shortcutDetails = shell.readShortcutLink(shortcutPath);

        electronSocket.emit('shell-readShortcutLinkCompleted', shortcutDetails);
    });
};
