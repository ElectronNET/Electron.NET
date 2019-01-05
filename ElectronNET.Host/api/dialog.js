"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('showMessageBox', (browserWindow, options, guid) => {
        if ('id' in browserWindow) {
            const window = electron_1.BrowserWindow.fromId(browserWindow.id);
            electron_1.dialog.showMessageBox(window, options, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete' + guid, [response, checkboxChecked]);
            });
        }
        else {
            const id = guid || options;
            electron_1.dialog.showMessageBox(browserWindow, (response, checkboxChecked) => {
                socket.emit('showMessageBoxComplete' + id, [response, checkboxChecked]);
            });
        }
    });
    socket.on('showOpenDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showOpenDialog(window, options, (filePaths) => {
            socket.emit('showOpenDialogComplete' + guid, filePaths || []);
        });
    });
    socket.on('showSaveDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showSaveDialog(window, options, (filename) => {
            socket.emit('showSaveDialogComplete' + guid, filename || '');
        });
    });
    socket.on('showErrorBox', (title, content) => {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showCertificateTrustDialog(window, options, () => {
            socket.emit('showCertificateTrustDialogComplete' + guid);
        });
    });
};
//# sourceMappingURL=dialog.js.map