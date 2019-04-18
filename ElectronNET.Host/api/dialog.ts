import { BrowserWindow, dialog } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('showMessageBox', (browserWindow, options, guid) => {
        if ('id' in browserWindow) {
            const window = BrowserWindow.fromId(browserWindow.id);

            dialog.showMessageBox(window, options, (response, checkboxChecked) => {
                electronSocket.emit('showMessageBoxComplete' + guid, [response, checkboxChecked]);
            });
        } else {
            const id = guid || options;
            dialog.showMessageBox(browserWindow, (response, checkboxChecked) => {
                electronSocket.emit('showMessageBoxComplete' + id, [response, checkboxChecked]);
            });
        }
    });

    socket.on('showOpenDialog', (browserWindow, options, guid) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        dialog.showOpenDialog(window, options, (filePaths) => {
            electronSocket.emit('showOpenDialogComplete' + guid, filePaths || []);
        });
    });

    socket.on('showSaveDialog', (browserWindow, options, guid) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        dialog.showSaveDialog(window, options, (filename) => {
            electronSocket.emit('showSaveDialogComplete' + guid, filename || '');
        });
    });

    socket.on('showErrorBox', (title, content) => {
        dialog.showErrorBox(title, content);
    });

    socket.on('showCertificateTrustDialog', (browserWindow, options, guid) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        dialog.showCertificateTrustDialog(window, options, () => {
            electronSocket.emit('showCertificateTrustDialogComplete' + guid);
        });
    });
};
