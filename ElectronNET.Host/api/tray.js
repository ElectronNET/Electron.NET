"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var path = require('path');
var tray;
module.exports = function (socket) {
    socket.on('create-tray', function (image, menuItems) {
        var menu = electron_1.Menu.buildFromTemplate(menuItems);
        var imagePath = path.join(__dirname.replace('api', ''), 'bin', image);
        tray = new electron_1.Tray(imagePath);
        tray.setContextMenu(menu);
    });
};
//# sourceMappingURL=tray.js.map