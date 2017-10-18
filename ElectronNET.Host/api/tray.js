"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var path = require('path');
var tray;
module.exports = function (socket) {
    socket.on('register-tray-click', function () {
        if (tray) {
            tray.on('click', function (event, bounds) {
                socket.emit('tray-click-event', [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-right-click', function () {
        if (tray) {
            tray.on('right-click', function (event, bounds) {
                socket.emit('tray-right-click-event', [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-double-click', function () {
        if (tray) {
            tray.on('double-click', function (event, bounds) {
                socket.emit('tray-double-click-event', [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-balloon-show', function () {
        if (tray) {
            tray.on('balloon-show', function () {
                socket.emit('tray-balloon-show-event');
            });
        }
    });
    socket.on('register-tray-balloon-click', function () {
        if (tray) {
            tray.on('balloon-click', function () {
                socket.emit('tray-balloon-click-event');
            });
        }
    });
    socket.on('register-tray-balloon-closed', function () {
        if (tray) {
            tray.on('balloon-closed', function () {
                socket.emit('tray-balloon-closed-event');
            });
        }
    });
    socket.on('create-tray', function (image, menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, function (id) {
            socket.emit("trayMenuItemClicked", id);
        });
        var imagePath = path.join(__dirname.replace('api', ''), 'bin', image);
        tray = new electron_1.Tray(imagePath);
        tray.setContextMenu(menu);
    });
    socket.on('tray-destroy', function () {
        if (tray) {
            tray.destroy();
        }
    });
    socket.on('tray-setImage', function (image) {
        if (tray) {
            tray.setImage(image);
        }
    });
    socket.on('tray-setPressedImage', function (image) {
        if (tray) {
            var img = electron_1.nativeImage.createFromPath(image);
            tray.setPressedImage(img);
        }
    });
    socket.on('tray-setToolTip', function (toolTip) {
        if (tray) {
            tray.setToolTip(toolTip);
        }
    });
    socket.on('tray-setTitle', function (title) {
        if (tray) {
            tray.setTitle(title);
        }
    });
    socket.on('tray-setHighlightMode', function (mode) {
        if (tray) {
            tray.setHighlightMode(mode);
        }
    });
    socket.on('tray-displayBalloon', function (options) {
        if (tray) {
            tray.displayBalloon(options);
        }
    });
    socket.on('tray-isDestroyed', function () {
        if (tray) {
            var isDestroyed = tray.isDestroyed();
            socket.emit('tray-isDestroyedCompleted', isDestroyed);
        }
    });
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach(function (item) {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ("id" in item && item.id) {
                item.click = function () { callback(item.id); };
            }
        });
    }
};
//# sourceMappingURL=tray.js.map