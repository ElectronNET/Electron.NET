"use strict";
var electron_1 = require("electron");
var tray = (global['tray'] = global['tray'] || { value: null });
var electronSocket;
module.exports = function (socket) {
    electronSocket = socket;
    socket.on('register-tray-click', function (id) {
        if (tray.value) {
            tray.value.on('click', function (event, bounds) {
                electronSocket.emit('tray-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-right-click', function (id) {
        if (tray.value) {
            tray.value.on('right-click', function (event, bounds) {
                electronSocket.emit('tray-right-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-double-click', function (id) {
        if (tray.value) {
            tray.value.on('double-click', function (event, bounds) {
                electronSocket.emit('tray-double-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-balloon-show', function (id) {
        if (tray.value) {
            tray.value.on('balloon-show', function () {
                electronSocket.emit('tray-balloon-show-event' + id);
            });
        }
    });
    socket.on('register-tray-balloon-click', function (id) {
        if (tray.value) {
            tray.value.on('balloon-click', function () {
                electronSocket.emit('tray-balloon-click-event' + id);
            });
        }
    });
    socket.on('register-tray-balloon-closed', function (id) {
        if (tray.value) {
            tray.value.on('balloon-closed', function () {
                electronSocket.emit('tray-balloon-closed-event' + id);
            });
        }
    });
    socket.on('create-tray', function (image, menuItems) {
        var trayIcon = electron_1.nativeImage.createFromPath(image);
        tray.value = new electron_1.Tray(trayIcon);
        if (menuItems) {
            var menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, function (id) {
                electronSocket.emit('trayMenuItemClicked', id);
            });
            tray.value.setContextMenu(menu);
        }
    });
    socket.on('tray-destroy', function () {
        if (tray.value) {
            tray.value.destroy();
        }
    });
    socket.on('tray-setImage', function (image) {
        if (tray.value) {
            tray.value.setImage(image);
        }
    });
    socket.on('tray-setPressedImage', function (image) {
        if (tray.value) {
            var img = electron_1.nativeImage.createFromPath(image);
            tray.value.setPressedImage(img);
        }
    });
    socket.on('tray-setToolTip', function (toolTip) {
        if (tray.value) {
            tray.value.setToolTip(toolTip);
        }
    });
    socket.on('tray-setTitle', function (title) {
        if (tray.value) {
            tray.value.setTitle(title);
        }
    });
    socket.on('tray-displayBalloon', function (options) {
        if (tray.value) {
            tray.value.displayBalloon(options);
        }
    });
    socket.on('tray-isDestroyed', function () {
        if (tray.value) {
            var isDestroyed = tray.value.isDestroyed();
            electronSocket.emit('tray-isDestroyedCompleted', isDestroyed);
        }
    });
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach(function (item) {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ('id' in item && item.id) {
                item.click = function () { callback(item.id); };
            }
        });
    }
};
