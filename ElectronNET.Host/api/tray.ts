import { Menu, Tray, nativeImage } from "electron";
const path = require('path');
let tray: Electron.Tray;

module.exports = (socket: SocketIO.Server) => {
    socket.on('register-tray-click', () => {
        if (tray) {
            tray.on('click', (event, bounds) => {
                socket.emit('tray-click-event', [(<any>event).__proto__, bounds]);
            });
        }
    });

    socket.on('register-tray-right-click', () => {
        if (tray) {
            tray.on('right-click', (event, bounds) => {
                socket.emit('tray-right-click-event', [(<any>event).__proto__, bounds]);
            });
        }
    });

    socket.on('register-tray-double-click', () => {
        if (tray) {
            tray.on('double-click', (event, bounds) => {
                socket.emit('tray-double-click-event', [(<any>event).__proto__, bounds]);
            });
        }
    });

    socket.on('register-tray-balloon-show', () => {
        if (tray) {
            tray.on('balloon-show', () => {
                socket.emit('tray-balloon-show-event');
            });
        }
    });
    
    socket.on('register-tray-balloon-click', () => {
        if (tray) {
            tray.on('balloon-click', () => {
                socket.emit('tray-balloon-click-event');
            });
        }
    });

    socket.on('register-tray-balloon-closed', () => {
        if (tray) {
            tray.on('balloon-closed', () => {
                socket.emit('tray-balloon-closed-event');
            });
        }
    });

    socket.on('create-tray', (image, menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        addMenuItemClickConnector(menu.items, (id) => {
            socket.emit("trayMenuItemClicked", id);
        });

        const imagePath = path.join(__dirname.replace('api', ''), 'bin', image);

        tray = new Tray(imagePath);
        tray.setContextMenu(menu);
    });

    socket.on('tray-destroy', () => {
        if (tray) {
            tray.destroy();
        }
    });

    socket.on('tray-setImage', (image) => {
        if (tray) {
            tray.setImage(image);
        }
    });

    socket.on('tray-setPressedImage', (image) => {
        if (tray) {
            let img = nativeImage.createFromPath(image);
            tray.setPressedImage(img);
        }
    });

    socket.on('tray-setToolTip', (toolTip) => {
        if (tray) {
            tray.setToolTip(toolTip);
        }
    });

    socket.on('tray-setTitle', (title) => {
        if (tray) {
            tray.setTitle(title);
        }
    });

    socket.on('tray-setHighlightMode', (mode) => {
        if (tray) {
            tray.setHighlightMode(mode);
        }
    });

    socket.on('tray-displayBalloon', (options) => {
        if (tray) {
            tray.displayBalloon(options);
        }
    });

    socket.on('tray-isDestroyed', () => {
        if (tray) {
            let isDestroyed = tray.isDestroyed();
            socket.emit('tray-isDestroyedCompleted', isDestroyed);
        }
    });

    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }

            if ("id" in item && item.id) {
                item.click = () => { callback(item.id); };
            }
        });
    }
}