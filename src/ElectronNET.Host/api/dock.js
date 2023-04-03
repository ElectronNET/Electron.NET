"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('dock-bounce', (type) => {
        const id = electron_1.app.dock.bounce(type);
        electronSocket.emit('dock-bounce-completed', id);
    });
    socket.on('dock-cancelBounce', (id) => {
        electron_1.app.dock.cancelBounce(id);
    });
    socket.on('dock-downloadFinished', (filePath) => {
        electron_1.app.dock.downloadFinished(filePath);
    });
    socket.on('dock-setBadge', (text) => {
        electron_1.app.dock.setBadge(text);
    });
    socket.on('dock-getBadge', () => {
        const text = electron_1.app.dock.getBadge();
        electronSocket.emit('dock-getBadge-completed', text);
    });
    socket.on('dock-hide', () => {
        electron_1.app.dock.hide();
    });
    socket.on('dock-show', () => {
        electron_1.app.dock.show();
    });
    socket.on('dock-isVisible', () => {
        const isVisible = electron_1.app.dock.isVisible();
        electronSocket.emit('dock-isVisible-completed', isVisible);
    });
    socket.on('dock-setMenu', (menuItems) => {
        let menu = null;
        if (menuItems) {
            menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                electronSocket.emit('dockMenuItemClicked', id);
            });
        }
        electron_1.app.dock.setMenu(menu);
    });
    // TODO: Menu (macOS) still to be implemented
    socket.on('dock-getMenu', () => {
        const menu = electron_1.app.dock.getMenu();
        electronSocket.emit('dock-getMenu-completed', menu);
    });
    socket.on('dock-setIcon', (image) => {
        electron_1.app.dock.setIcon(image);
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
//# sourceMappingURL=dock.js.map