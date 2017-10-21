import { BrowserWindow } from 'electron';
module.exports = (socket: SocketIO.Server) => {
    socket.on('register-webContents-crashed', (id) => {
        var browserWindow = getWindowById(id);

        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            socket.emit('webContents-crashed' + id, killed);
        });
    });

    socket.on('register-webContents-didFinishLoad', (id) => {
        var browserWindow = getWindowById(id);

        browserWindow.webContents.removeAllListeners('did-finish-load');
        browserWindow.webContents.on('did-finish-load', () => {
            socket.emit('webContents-didFinishLoad' + id);
        });
    });

    socket.on('webContentsOpenDevTools', (id, options) => {
        if (options) {
            getWindowById(id).webContents.openDevTools(options);
        } else {
            getWindowById(id).webContents.openDevTools();
        }
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        return BrowserWindow.fromId(id);
    }
};