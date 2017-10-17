"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('showMessageBox', function (browserWindow, options) {
        if ("id" in browserWindow) {
            var window = electron_1.BrowserWindow.fromId(browserWindow.id);
            electron_1.dialog.showMessageBox(window, options, function (response, checkboxChecked) {
                socket.emit('showMessageBoxComplete', [response, checkboxChecked]);
            });
        }
        else {
            electron_1.dialog.showMessageBox(browserWindow, function (response, checkboxChecked) {
                socket.emit('showMessageBoxComplete', [response, checkboxChecked]);
            });
        }
    });
    socket.on('showOpenDialog', function (browserWindow, options) {
        var window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showOpenDialog(window, options, function (filePaths) {
            socket.emit('showOpenDialogComplete', filePaths || []);
        });
    });
    socket.on('showSaveDialog', function (browserWindow, options) {
        var window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showSaveDialog(window, options, function (filename) {
            socket.emit('showSaveDialogComplete', filename || '');
        });
    });
    socket.on('showErrorBox', function (title, content) {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', function (browserWindow, options) {
        var window = electron_1.BrowserWindow.fromId(browserWindow.id);
        electron_1.dialog.showCertificateTrustDialog(window, options, function () {
            socket.emit('showCertificateTrustDialogComplete');
        });
    });
};
//# sourceMappingURL=dialog.js.map