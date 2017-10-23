"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var fs = require('fs');
module.exports = function (socket) {
    socket.on('register-webContents-crashed', function (id) {
        var browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', function (event, killed) {
            socket.emit('webContents-crashed' + id, killed);
        });
    });
    socket.on('register-webContents-didFinishLoad', function (id) {
        var browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('did-finish-load');
        browserWindow.webContents.on('did-finish-load', function () {
            socket.emit('webContents-didFinishLoad' + id);
        });
    });
    socket.on('webContentsOpenDevTools', function (id, options) {
        if (options) {
            getWindowById(id).webContents.openDevTools(options);
        }
        else {
            getWindowById(id).webContents.openDevTools();
        }
    });
    socket.on('webContents-printToPDF', function (id, options, path) {
        getWindowById(id).webContents.printToPDF(options || {}, function (error, data) {
            if (error) {
                throw error;
            }
            fs.writeFile(path, data, function (error) {
                if (error) {
                    socket.emit('webContents-printToPDF-completed', false);
                }
                else {
                    socket.emit('webContents-printToPDF-completed', true);
                }
            });
        });
    });
    function getWindowById(id) {
        return electron_1.BrowserWindow.fromId(id);
    }
};
//# sourceMappingURL=webContents.js.map