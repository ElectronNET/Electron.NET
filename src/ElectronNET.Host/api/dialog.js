"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('showMessageBox', async (browserWindow, options, guid) => {
        if ('id' in browserWindow) {
            const window = electron_1.BrowserWindow.fromId(browserWindow.id);
            const messageBoxReturnValue = await electron_1.dialog.showMessageBox(window, options);
            electronSocket.emit('showMessageBoxComplete' + guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
        else {
            const id = guid || options;
            const messageBoxReturnValue = await electron_1.dialog.showMessageBox(browserWindow);
            electronSocket.emit('showMessageBoxComplete' + id, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
    });
    socket.on('showOpenDialog', async (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const openDialogReturnValue = await electron_1.dialog.showOpenDialog(window, options);
        electronSocket.emit('showOpenDialogComplete' + guid, openDialogReturnValue.filePaths || []);
    });
    socket.on('showSaveDialog', async (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const saveDialogReturnValue = await electron_1.dialog.showSaveDialog(window, options);
        electronSocket.emit('showSaveDialogComplete' + guid, saveDialogReturnValue.filePath || '');
    });
    socket.on('showErrorBox', (title, content) => {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', async (browserWindow, options, guid) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        await electron_1.dialog.showCertificateTrustDialog(window, options);
        electronSocket.emit('showCertificateTrustDialogComplete' + guid);
    });
};
//# sourceMappingURL=dialog.js.map