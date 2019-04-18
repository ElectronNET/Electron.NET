"use strict";
const electron_1 = require("electron");
const fs = require('fs');
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('register-webContents-crashed', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            electronSocket.emit('webContents-crashed' + id, killed);
        });
    });
    socket.on('register-webContents-didFinishLoad', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('did-finish-load');
        browserWindow.webContents.on('did-finish-load', () => {
            electronSocket.emit('webContents-didFinishLoad' + id);
        });
    });
    socket.on('webContentsOpenDevTools', (id, options) => {
        if (options) {
            getWindowById(id).webContents.openDevTools(options);
        }
        else {
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
                    electronSocket.emit('webContents-printToPDF-completed', false);
                }
                else {
                    electronSocket.emit('webContents-printToPDF-completed', true);
                }
            });
        });
    });
    socket.on('webContents-getUrl', function (id) {
        const browserWindow = getWindowById(id);
        electronSocket.emit('webContents-getUrl' + id, browserWindow.webContents.getURL());
    });
    function getWindowById(id) {
        return electron_1.BrowserWindow.fromId(id);
    }
};
//# sourceMappingURL=webContents.js.map