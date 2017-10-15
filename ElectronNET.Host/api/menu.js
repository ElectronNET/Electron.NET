"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('menu-setApplicationMenu', function (menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, function (id) {
            socket.emit("menuItemClicked", id);
        });
        electron_1.Menu.setApplicationMenu(menu);
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
//# sourceMappingURL=menu.js.map