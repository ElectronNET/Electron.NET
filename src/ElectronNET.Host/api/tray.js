"use strict";
const electron_1 = require("electron");
let tray = (global["$tray"] = global["tray"] || {
    value: null,
});
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on("register-tray-click", (id) => {
        if (tray.value) {
            tray.value.on("click", (event, bounds) => {
                electronSocket.emit("tray-click" + id, [
                    event.__proto__,
                    bounds,
                ]);
            });
        }
    });
    socket.on("register-tray-right-click", (id) => {
        if (tray.value) {
            tray.value.on("right-click", (event, bounds) => {
                electronSocket.emit("tray-right-click" + id, [
                    event.__proto__,
                    bounds,
                ]);
            });
        }
    });
    socket.on("register-tray-double-click", (id) => {
        if (tray.value) {
            tray.value.on("double-click", (event, bounds) => {
                electronSocket.emit("tray-double-click" + id, [
                    event.__proto__,
                    bounds,
                ]);
            });
        }
    });
    socket.on("register-tray-balloon-show", (id) => {
        if (tray.value) {
            tray.value.on("balloon-show", () => {
                electronSocket.emit("tray-balloon-show" + id);
            });
        }
    });
    socket.on("register-tray-balloon-click", (id) => {
        if (tray.value) {
            tray.value.on("balloon-click", () => {
                electronSocket.emit("tray-balloon-click" + id);
            });
        }
    });
    socket.on("register-tray-balloon-closed", (id) => {
        if (tray.value) {
            tray.value.on("balloon-closed", () => {
                electronSocket.emit("tray-balloon-closed" + id);
            });
        }
    });
    socket.on("create-tray", (image, menuItems) => {
        const trayIcon = electron_1.nativeImage.createFromPath(image);
        tray.value = new electron_1.Tray(trayIcon);
        if (menuItems) {
            applyContextMenu(menuItems);
        }
    });
    socket.on("tray-destroy", () => {
        if (tray.value) {
            tray.value.destroy();
        }
    });
    socket.on("set-contextMenu", (menuItems) => {
        if (menuItems && tray.value) {
            applyContextMenu(menuItems);
        }
    });
    socket.on("tray-setImage", (image) => {
        if (tray.value) {
            tray.value.setImage(image);
        }
    });
    socket.on("tray-setPressedImage", (image) => {
        if (tray.value) {
            const img = electron_1.nativeImage.createFromPath(image);
            tray.value.setPressedImage(img);
        }
    });
    socket.on("tray-setToolTip", (toolTip) => {
        if (tray.value) {
            tray.value.setToolTip(toolTip);
        }
    });
    socket.on("tray-setTitle", (title) => {
        if (tray.value) {
            tray.value.setTitle(title);
        }
    });
    socket.on("tray-displayBalloon", (options) => {
        if (tray.value) {
            tray.value.displayBalloon(options);
        }
    });
    socket.on("tray-isDestroyed", () => {
        if (tray.value) {
            const isDestroyed = tray.value.isDestroyed();
            electronSocket.emit("tray-isDestroyedCompleted", isDestroyed);
        }
    });
    socket.on("register-tray-on-event", (eventName, listenerName) => {
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
    socket.on("register-tray-once-event", (eventName, listenerName) => {
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
    function applyContextMenu(menuItems) {
        const menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, (id) => {
            electronSocket.emit("trayMenuItemClicked", id);
        });
        tray.value.setContextMenu(menu);
    }
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ("id" in item && item.id) {
                item.click = () => {
                    callback(item.id);
                };
            }
        });
    }
};
//# sourceMappingURL=tray.js.map