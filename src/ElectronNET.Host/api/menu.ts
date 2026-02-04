import type { Socket } from "net";
import { Menu, BrowserWindow } from "electron";

const contextMenuItems = (global["contextMenuItems"] =
  global["contextMenuItems"] || []);

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;
  socket.on("menu-setContextMenu", (browserWindowId, menuItems) => {
    const menu = Menu.buildFromTemplate(menuItems);

    addContextMenuItemClickConnector(
      menu.items,
      browserWindowId,
      (id, windowId) => {
        electronSocket.emit("contextMenuItemClicked", [id, windowId]);
      },
    );

    const index = contextMenuItems.findIndex(
      (contextMenu) => contextMenu.browserWindowId === browserWindowId,
    );

    const contextMenuItem = {
      menu: menu,
      browserWindowId: browserWindowId,
    };

    if (index === -1) {
      contextMenuItems.push(contextMenuItem);
    } else {
      contextMenuItems[index] = contextMenuItem;
    }
  });

  function addContextMenuItemClickConnector(
    menuItems,
    browserWindowId,
    callback,
  ) {
    menuItems.forEach((item) => {
      if (item.submenu && item.submenu.items.length > 0) {
        addContextMenuItemClickConnector(
          item.submenu.items,
          browserWindowId,
          callback,
        );
      }

      if ("id" in item && item.id) {
        item.click = () => {
          callback(item.id, browserWindowId);
        };
      }
    });
  }

  socket.on("menu-contextMenuPopup", (browserWindowId) => {
    contextMenuItems.forEach((x) => {
      if (x.browserWindowId === browserWindowId) {
        const browserWindow = BrowserWindow.fromId(browserWindowId);
        x.menu.popup(browserWindow);
      }
    });
  });

  socket.on("menu-setApplicationMenu", (menuItems) => {
    const menu = Menu.buildFromTemplate(menuItems);

    addMenuItemClickConnector(menu.items, (id) => {
      electronSocket.emit("menuItemClicked", id);
    });

    Menu.setApplicationMenu(menu);
  });

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
