"use strict";
const electron_1 = require("electron");
let tray = (global['$tray'] = global['tray'] || { value: null });
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('register-tray-click', (id) => {
        if (tray.value) {
            tray.value.on('click', (event, bounds) => {
                electronSocket.emit('tray-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-right-click', (id) => {
        if (tray.value) {
            tray.value.on('right-click', (event, bounds) => {
                electronSocket.emit('tray-right-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-double-click', (id) => {
        if (tray.value) {
            tray.value.on('double-click', (event, bounds) => {
                electronSocket.emit('tray-double-click-event' + id, [event.__proto__, bounds]);
            });
        }
    });
    socket.on('register-tray-balloon-show', (id) => {
        if (tray.value) {
            tray.value.on('balloon-show', () => {
                electronSocket.emit('tray-balloon-show-event' + id);
            });
        }
    });
    socket.on('register-tray-balloon-click', (id) => {
        if (tray.value) {
            tray.value.on('balloon-click', () => {
                electronSocket.emit('tray-balloon-click-event' + id);
            });
        }
    });
    socket.on('register-tray-balloon-closed', (id) => {
        if (tray.value) {
            tray.value.on('balloon-closed', () => {
                electronSocket.emit('tray-balloon-closed-event' + id);
            });
        }
    });
    socket.on('create-tray', (image, menuItems) => {
        const trayIcon = electron_1.nativeImage.createFromPath(image);
        tray.value = new electron_1.Tray(trayIcon);
        if (menuItems) {
            const menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                electronSocket.emit('trayMenuItemClicked', id);
            });
            tray.value.setContextMenu(menu);
        }
    });
    socket.on('tray-destroy', () => {
        if (tray.value) {
            tray.value.destroy();
        }
    });
    socket.on('tray-setImage', (image) => {
        if (tray.value) {
            tray.value.setImage(image);
        }
    });
    socket.on('tray-setPressedImage', (image) => {
        if (tray.value) {
            const img = electron_1.nativeImage.createFromPath(image);
            tray.value.setPressedImage(img);
        }
    });
    socket.on('tray-setToolTip', (toolTip) => {
        if (tray.value) {
            tray.value.setToolTip(toolTip);
        }
    });
    socket.on('tray-setTitle', (title) => {
        if (tray.value) {
            tray.value.setTitle(title);
        }
    });
    socket.on('tray-displayBalloon', (options) => {
        if (tray.value) {
            tray.value.displayBalloon(options);
        }
    });
    socket.on('tray-isDestroyed', () => {
        if (tray.value) {
            const isDestroyed = tray.value.isDestroyed();
            electronSocket.emit('tray-isDestroyedCompleted', isDestroyed);
        }
    });
    socket.on('register-tray-on-event', (eventName, listenerName) => {
        if (tray.value) {
            tray.value.on(eventName, (...args) => {
                if (args.length > 1) {
                    electronSocket.emit(listenerName, args[1]);
                }
                else {
                    electronSocket.emit(listenerName);
                }
            });
        }
    });
    socket.on('register-tray-once-event', (eventName, listenerName) => {
        if (tray.value) {
            tray.value.once(eventName, (...args) => {
                if (args.length > 1) {
                    electronSocket.emit(listenerName, args[1]);
                }
                else {
                    electronSocket.emit(listenerName);
                }
            });
        }
    });
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ('id' in item && item.id) {
                item.click = () => { callback(item.id); };
            }
        });
    }
};
//# sourceMappingURL=tray.js.map