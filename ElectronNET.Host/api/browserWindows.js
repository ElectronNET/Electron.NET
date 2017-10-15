"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var windows = [];
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
        if (loadUrl) {
            window.loadURL(loadUrl);
        }
        windows.push(window);
        socket.emit('BrowserWindowCreated', window.id);
    });
    socket.on('browserWindow-destroy', function (id) {
        getWindowById(id).destroy();
    });
    socket.on('browserWindow-close', function (id) {
        getWindowById(id).close();
    });
    socket.on('browserWindow-focus', function (id) {
        getWindowById(id).focus();
    });
    socket.on('browserWindow-blur', function (id) {
        getWindowById(id).blur();
    });
    socket.on('browserWindow-isFocused', function (id) {
        var isFocused = getWindowById(id).isFocused();
        socket.emit('browserWindow-isFocused-completed', isFocused);
    });
    socket.on('browserWindow-isDestroyed', function (id) {
        var isDestroyed = getWindowById(id).isDestroyed();
        socket.emit('browserWindow-isDestroyed-completed', isDestroyed);
    });
    socket.on('browserWindow-show', function (id) {
        getWindowById(id).show();
    });
    socket.on('browserWindow-showInactive', function (id) {
        getWindowById(id).showInactive();
    });
    socket.on('browserWindow-hide', function (id) {
        getWindowById(id).hide();
    });
    socket.on('browserWindow-isVisible', function (id) {
        var isVisible = getWindowById(id).isVisible();
        socket.emit('browserWindow-isVisible-completed', isVisible);
    });
    socket.on('browserWindow-isModal', function (id) {
        var isModal = getWindowById(id).isModal();
        socket.emit('browserWindow-isModal-completed', isModal);
    });
    socket.on('browserWindow-maximize', function (id) {
        getWindowById(id).maximize();
    });
    socket.on('browserWindow-unmaximize', function (id) {
        getWindowById(id).unmaximize();
    });
    socket.on('browserWindow-isMaximized', function (id) {
        var isMaximized = getWindowById(id).isMaximized();
        socket.emit('browserWindow-isMaximized-completed', isMaximized);
    });
    socket.on('browserWindow-minimize', function (id) {
        getWindowById(id).minimize();
    });
    socket.on('browserWindow-restore', function (id) {
        getWindowById(id).restore();
    });
    socket.on('browserWindow-isMinimized', function (id) {
        var isMinimized = getWindowById(id).isMinimized();
        socket.emit('browserWindow-isMinimized-completed', isMinimized);
    });
    socket.on('browserWindow-setFullScreen', function (id, fullscreen) {
        getWindowById(id).setFullScreen(fullscreen);
    });
    socket.on('browserWindow-isFullScreen', function (id) {
        var isFullScreen = getWindowById(id).isFullScreen();
        socket.emit('browserWindow-isFullScreen-completed', isFullScreen);
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