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

    socket.on('shell-openExternal', (url, options) => {
        let success = true;

        if (options) {
            shell.openExternal(url, options).catch((error) => {
                success = false;
                electronSocket.emit('shell-openExternalCallback', [url, error]);
            });
        } else {
            shell.openExternal(url).catch((error) => {
                success = false;
                electronSocket.emit('shell-openExternalCallback', [url, error]);
            });
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
