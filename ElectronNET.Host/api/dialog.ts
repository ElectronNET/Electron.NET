import { BrowserWindow, dialog } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('showMessageBox', (browserWindow, options, guid) => {
        if ("id" in browserWindow) {
            var window = BrowserWindow.fromId(browserWindow.id);

            dialog.showMessageBox(window, options, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete' + guid, [response, checkboxChecked]);
            });
        } else {
            var message = browserWindow;
            let id = guid || options;
            dialog.showMessageBox(browserWindow, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete' + id, [response, checkboxChecked]);
            });
        }
    });

    socket.on('showOpenDialog', (browserWindow, options, guid) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showOpenDialog(window, options, (filePaths) => {
            socket.emit('showOpenDialogComplete' + guid, filePaths || []);
        });
    });

    socket.on('showSaveDialog', (browserWindow, options, guid) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showSaveDialog(window, options, (filename) => {
            socket.emit('showSaveDialogComplete' + guid, filename || '');
        });
    });

    socket.on('showErrorBox', (title, content) => {
        dialog.showErrorBox(title, content);
    });

    socket.on('showCertificateTrustDialog', (browserWindow, options, guid) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showCertificateTrustDialog(window, options, () => {
            socket.emit('showCertificateTrustDialogComplete' + guid);
        });
    });
}