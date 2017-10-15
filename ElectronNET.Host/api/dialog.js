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
};
//# sourceMappingURL=dialog.js.map