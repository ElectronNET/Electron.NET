import { Menu, BrowserWindow } from "electron";
const contextMenuItems = [];

module.exports = (socket: SocketIO.Server) => {
    socket.on('menu-setContextMenu', (browserWindowId, menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        addContextMenuItemClickConnector(menu.items, browserWindowId, (id, browserWindowId) => {
            socket.emit("contextMenuItemClicked", [id, browserWindowId]);
        });

        contextMenuItems.push({
            menu: menu,
            browserWindowId: browserWindowId
        });
    });

    function addContextMenuItemClickConnector(menuItems, browserWindowId, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addContextMenuItemClickConnector(item.submenu.items, browserWindowId, callback);
            }

            if ("id" in item && item.id) {
                item.click = () => { callback(item.id, browserWindowId); };
            }
        });
    }

    socket.on('menu-contextMenuPopup', (browserWindowId) => {
        contextMenuItems.forEach(x => {
            if(x.browserWindowId === browserWindowId) {
                let browserWindow = BrowserWindow.fromId(browserWindowId);
                x.menu.popup(browserWindow);
            }
        });
    });

    socket.on('menu-setApplicationMenu', (menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        addMenuItemClickConnector(menu.items, (id) => {
            socket.emit("menuItemClicked", id);
        });

        Menu.setApplicationMenu(menu);
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