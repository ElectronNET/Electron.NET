import { app, Menu } from 'electron';

export = (socket: SignalR.Hub.Proxy) => {

    socket.on('dock-bounce', (guid, type) => {
        const id = app.dock.bounce(type);
        socket.invoke('SendClientResponseInt', guid, id);
    });

    socket.on('dock-cancelBounce', (id) => {
        app.dock.cancelBounce(id);
    });

    socket.on('dock-downloadFinished', (filePath) => {
        app.dock.downloadFinished(filePath);
    });

    socket.on('dock-setBadge', (text) => {
        app.dock.setBadge(text);
    });

    socket.on('dock-getBadge', (guid) => {
        const text = app.dock.getBadge();
        socket.invoke('SendClientResponseString', guid, text);
    });

    socket.on('dock-hide', () => {
        app.dock.hide();
    });

    socket.on('dock-show', () => {
        app.dock.show();
    });

    socket.on('dock-isVisible', (guid) => {
        const isVisible = app.dock.isVisible();
        socket.invoke('SendClientResponseBool', guid, isVisible);
    });

    socket.on('dock-setMenu', (menuItems) => {
        let menu = null;

        if (menuItems) {
            menu = Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                socket.invoke('DockMenuItemClicked', id);
            });
        }

        app.dock.setMenu(menu);
    });

    // TODO: Menu (macOS) still to be implemented
    socket.on('dock-getMenu', (guid) => {
        const menu = app.dock.getMenu();
        socket.invoke('SendClientResponseJObject', guid, menu);
    });

    socket.on('dock-setIcon', (image) => {
        app.dock.setIcon(image);
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
