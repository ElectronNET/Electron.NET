import { Menu, Tray } from "electron";
const path = require('path');
let tray;

module.exports = (socket: SocketIO.Server) => {
    socket.on('create-tray', (image, menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        const imagePath = path.join(__dirname.replace('api', ''), 'bin', image);

        tray = new Tray(imagePath);
        tray.setContextMenu(menu);
    });
}