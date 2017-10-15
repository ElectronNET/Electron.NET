"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var path = require('path');
var tray;
module.exports = function (socket) {
    socket.on('create-tray', function (image, menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        addMenuItemClickConnector(menu.items, function (id) {
            socket.emit("trayMenuItemClicked", id);
        });
        var imagePath = path.join(__dirname.replace('api', ''), 'bin', image);
        tray = new electron_1.Tray(imagePath);
        tray.setContextMenu(menu);
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