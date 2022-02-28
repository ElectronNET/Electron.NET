import { BrowserWindow, dialog } from 'electron';

export = (socket: SignalR.Hub.Proxy) => {
    socket.on('showMessageBox', async (guid, browserWindow, options) => {
        if ('id' in browserWindow) {
            const window = BrowserWindow.fromId(browserWindow.id);

            const messageBoxReturnValue = await dialog.showMessageBox(window, options);
                socket.invoke('SendClientResponseJArray', guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        } else {
            const messageBoxReturnValue = await dialog.showMessageBox(browserWindow);

                socket.invoke('SendClientResponseJArray', guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
    });

    socket.on('showOpenDialog', async (guid, browserWindow, options) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        const openDialogReturnValue = await dialog.showOpenDialog(window, options);

            socket.invoke('SendClientResponseJArray', guid, openDialogReturnValue.filePaths || []);
    });

    socket.on('showSaveDialog', async (guid, browserWindow, options) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        const saveDialogReturnValue = await dialog.showSaveDialog(window, options);

            socket.invoke('SendClientResponseString', guid, saveDialogReturnValue.filePath || '');
    });

    socket.on('showErrorBox', (title, content) => {
        dialog.showErrorBox(title, content);
    });

    socket.on('showCertificateTrustDialog', async (guid, browserWindow, options) => {
        const window = BrowserWindow.fromId(browserWindow.id);
        await dialog.showCertificateTrustDialog(window, options);
            // Needed ?
            //socket.invoke('showCertificateTrustDialogComplete', guid);
    });
};
