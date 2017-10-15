import { Menu } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('menu-setApplicationMenu', (menuItems) => {
        const menu = Menu.buildFromTemplate(menuItems);

        addMenuItemClickConnector(menu.items, (id) => {
            socket.emit("menuItemClicked", id);
        });

        Menu.setApplicationMenu(menu);
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