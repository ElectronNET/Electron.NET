"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('showMessageBox', (browserWindow, options, guid) => {
        if ('id' in browserWindow) {
            const window = electron_1.BrowserWindow.fromId(browserWindow.id);
            electron_1.dialog.showMessageBox(window, options, (response, checkboxChecked) => {
                electronSocket.emit('showMessageBoxComplete' + guid, [response, checkboxChecked]);
            });
        }
        else {
            const id = guid || options;
            electron_1.dialog.showMessageBox(browserWindow, (response, checkboxChecked) => {
                electronSocket.emit('showMessageBoxComplete' + id, [response, checkboxChecked]);
            });
        }
    });
    socket.on('showOpenDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showOpenDialog(window, options, (filePaths) => {
            electronSocket.emit('showOpenDialogComplete' + guid, filePaths || []);
        });
    });
    socket.on('showSaveDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showSaveDialog(window, options, (filename) => {
            electronSocket.emit('showSaveDialogComplete' + guid, filename || '');
        });
    });
    socket.on('showErrorBox', (title, content) => {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showCertificateTrustDialog(window, options, () => {
            electronSocket.emit('showCertificateTrustDialogComplete' + guid);
        });
    });
};
//# sourceMappingURL=dialog.js.map