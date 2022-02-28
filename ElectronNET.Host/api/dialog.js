"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('showMessageBox', async (guid, browserWindow, options) => {
        if ('id' in browserWindow) {
            const window = electron_1.BrowserWindow.fromId(browserWindow.id);
            const messageBoxReturnValue = await electron_1.dialog.showMessageBox(window, options);
            socket.invoke('SendClientResponseJArray', guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
        else {
            const messageBoxReturnValue = await electron_1.dialog.showMessageBox(browserWindow);
            socket.invoke('SendClientResponseJArray', guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
    });
    socket.on('showOpenDialog', async (guid, browserWindow, options) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const openDialogReturnValue = await electron_1.dialog.showOpenDialog(window, options);
        socket.invoke('SendClientResponseJArray', guid, openDialogReturnValue.filePaths || []);
    });
    socket.on('showSaveDialog', async (guid, browserWindow, options) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const saveDialogReturnValue = await electron_1.dialog.showSaveDialog(window, options);
        socket.invoke('SendClientResponseString', guid, saveDialogReturnValue.filePath || '');
    });
    socket.on('showErrorBox', (title, content) => {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', async (guid, browserWindow, options) => {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        await electron_1.dialog.showCertificateTrustDialog(window, options);
        // Needed ?
        //socket.invoke('showCertificateTrustDialogComplete', guid);
    });
};
//# sourceMappingURL=dialog.js.map