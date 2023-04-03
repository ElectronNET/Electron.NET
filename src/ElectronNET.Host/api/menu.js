"use strict";
const electron_1 = require("electron");
const contextMenuItems = (global['contextMenuItems'] = global['contextMenuItems'] || []);
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('menu-setContextMenu', (browserWindowId, menuItems) => {
        const menu = electron_1.Menu.buildFromTemplate(menuItems);
        addContextMenuItemClickConnector(menu.items, browserWindowId, (id, windowId) => {
            electronSocket.emit('contextMenuItemClicked', [id, windowId]);
        });
        const index = contextMenuItems.findIndex(contextMenu => contextMenu.browserWindowId === browserWindowId);
        const contextMenuItem = {
            menu: menu,
            browserWindowId: browserWindowId
        };
        if (index === -1) {
            contextMenuItems.push(contextMenuItem);
        }
        else {
            contextMenuItems[index] = contextMenuItem;
        }
    });
    function addContextMenuItemClickConnector(menuItems, browserWindowId, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addContextMenuItemClickConnector(item.submenu.items, browserWindowId, callback);
            }
            if ('id' in item && item.id) {
                item.click = () => { callback(item.id, browserWindowId); };
            }
        });
    }
    socket.on('menu-contextMenuPopup', (browserWindowId) => {
        contextMenuItems.forEach(x => {
            if (x.browserWindowId === browserWindowId) {
                const browserWindow = electron_1.BrowserWindow.fromId(browserWindowId);
                x.menu.popup(browserWindow);
            }
        });
    });
    socket.on('menu-setApplicationMenu', (menuItems) => {
        const menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, (id) => {
            electronSocket.emit('menuItemClicked', id);
        });
        electron_1.Menu.setApplicationMenu(menu);
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
//# sourceMappingURL=menu.js.map