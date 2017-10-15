import { BrowserWindow, dialog } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('showMessageBox', (browserWindow, options) => {
        if("id" in browserWindow) {
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
}