import { BrowserWindow, dialog } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('showMessageBox', (browserWindow, options) => {
        if ("id" in browserWindow) {
            var window = BrowserWindow.fromId(browserWindow.id);

            dialog.showMessageBox(window, options, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete', [response, checkboxChecked]);
            });
        } else {
            dialog.showMessageBox(browserWindow, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete', [response, checkboxChecked]);
            });
        }
    });

    socket.on('showOpenDialog', (browserWindow, options) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showOpenDialog(window, options, (filePaths) => {
            socket.emit('showOpenDialogComplete', filePaths || []);
        });
    });

    socket.on('showSaveDialog', (browserWindow, options) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showSaveDialog(window, options, (filename) => {
            socket.emit('showSaveDialogComplete', filename || '');
        });
    });

    socket.on('showErrorBox', (title, content) => {
        dialog.showErrorBox(title, content);
    });

    socket.on('showCertificateTrustDialog', (browserWindow, options) => {
        var window = BrowserWindow.fromId(browserWindow.id);
        dialog.showCertificateTrustDialog(window, options, () => {
            socket.emit('showCertificateTrustDialogComplete');
        });
    });
}