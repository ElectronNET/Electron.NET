import { BrowserWindow } from 'electron';
const fs = require('fs');

module.exports = (socket: SocketIO.Server) => {
    socket.on('register-webContents-crashed', (id) => {
        var browserWindow = getWindowById(id);

        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            socket.emit('webContents-crashed' + id, killed);
        });
    });

    socket.on('register-webContents-didFinishLoad', (id) => {
        let browserWindow = getWindowById(id);

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

    socket.on('webContents-printToPDF', (id, options, path) => {
        getWindowById(id).webContents.printToPDF(options || {}, (error, data) => {
            if (error) {
                throw error;
            }

            fs.writeFile(path, data, (error) => {
              if (error) {
                socket.emit('webContents-printToPDF-completed', false);
              } else {
                socket.emit('webContents-printToPDF-completed', true);
              }
            });
        });
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        return BrowserWindow.fromId(id);
    }
};