import { app } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;    

    socket.on('dock-bounce', (type) => {
        const id = app.dock.bounce(type);
        electronSocket.emit('dock-bounce-completed', id);
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

    socket.on('dock-getBadge', () => {
        const text = app.dock.getBadge();
        electronSocket.emit('dock-getBadge-completed', text);
    });

    socket.on('dock-hide', () => {
        app.dock.hide();
    });

    socket.on('dock-show', () => {
        app.dock.show();
    });

    socket.on('dock-isVisible', () => {
        const isVisible = app.dock.isVisible();
        electronSocket.emit('dock-isVisible-completed', isVisible);
    });

    // TODO: Menu (macOS) still to be implemented
    socket.on('dock-setMenu', (menu) => {
        app.dock.setMenu(menu);
    });

    // TODO: Menu (macOS) still to be implemented
    socket.on('dock-getMenu', () => {
        const menu = app.dock.getMenu();
        electronSocket.emit('dock-getMenu-completed', menu);
    });

    socket.on('dock-setIcon', (image) => {
        app.dock.setIcon(image);
    });
};