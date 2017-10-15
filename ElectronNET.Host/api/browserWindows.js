"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var windows = [];
var ipc;
module.exports = function (socket) {
    socket.on('createBrowserWindow', function (options, loadUrl) {
        var window = new electron_1.BrowserWindow(options);
        window.on('closed', function (sender) {
            for (var index = 0; index < windows.length; index++) {
                var windowItem = windows[index];
                try {
                    windowItem.id;
                }
                catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);
                    }
                }
            }
        });
        // TODO: IPC Lösung für mehrere Fenster finden 
        if (ipc == undefined) {
            ipc = require('./ipc')(socket, window);
        }
        if (loadUrl) {
            window.loadURL(loadUrl);
        }
        windows.push(window);
        socket.emit('BrowserWindowCreated', window.id);
    });
    socket.on('browserWindow-minimize', function (id) {
        getWindowById(id).minimize();
    });
    function getWindowById(id) {
        for (var index = 0; index < windows.length; index++) {
            var element = windows[index];
            if (element.id == id) {
                return element;
            }
        }
    }
};
//# sourceMappingURL=browserWindows.js.map