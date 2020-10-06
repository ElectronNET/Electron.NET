"use strict";
var electron_1 = require("electron");
var contextMenuItems = (global['contextMenuItems'] = global['contextMenuItems'] || []);
var electronSocket;
module.exports = function (socket) {
    electronSocket = socket;
    socket.on('menu-setContextMenu', function (browserWindowId, menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        addContextMenuItemClickConnector(menu.items, browserWindowId, function (id, browserWindowId) {
            electronSocket.emit('contextMenuItemClicked', [id, browserWindowId]);
        });
        var index = contextMenuItems.findIndex(function (contextMenu) { return contextMenu.browserWindowId === browserWindowId; });
        var contextMenuItem = {
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
        menuItems.forEach(function (item) {
            if (item.submenu && item.submenu.items.length > 0) {
                addContextMenuItemClickConnector(item.submenu.items, browserWindowId, callback);
            }
            if ('id' in item && item.id) {
                item.click = function () { callback(item.id, browserWindowId); };
            }
        });
    }
    socket.on('menu-contextMenuPopup', function (browserWindowId) {
        contextMenuItems.forEach(function (x) {
            if (x.browserWindowId === browserWindowId) {
                var browserWindow = electron_1.BrowserWindow.fromId(browserWindowId);
                x.menu.popup(browserWindow);
            }
        });
    });
    socket.on('menu-setApplicationMenu', function (menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, function (id) {
            electronSocket.emit('menuItemClicked', id);
        });
        electron_1.Menu.setApplicationMenu(menu);
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
