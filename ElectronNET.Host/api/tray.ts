import { Menu, Tray } from "electron";
const path = require('path');
let tray;

module.exports = (socket: SocketIO.Server) => {
    socket.on('create-tray', (image, menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        addMenuItemClickConnector(menu.items, (id) => {
            socket.emit("trayMenuItemClicked", id);
        });

        const imagePath = path.join(__dirname.replace('api', ''), 'bin', image);

        tray = new Tray(imagePath);
        tray.setContextMenu(menu);
    });

    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if(item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
    
            if("id" in item && item.id) {
                item.click = () => { callback(item.id); };
            }
        });
    }
}