import { BrowserWindow } from 'electron';
const fs = require('fs');

export = (socket: SocketIO.Socket) => {
    socket.on('register-webContents-crashed', (id) => {
        const browserWindow = getWindowById(id);

        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            socket.emit('webContents-crashed' + id, killed);
        });
    });

    socket.on('register-webContents-didFinishLoad', (id) => {
        const browserWindow = getWindowById(id);

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

    socket.on('webContents-getUrl', function (id) {
        const browserWindow = getWindowById(id);
        socket.emit('webContents-getUrl' + id, browserWindow.webContents.getURL());
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        return BrowserWindow.fromId(id);
    }
};
