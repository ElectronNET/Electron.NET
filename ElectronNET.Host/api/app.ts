import {} from 'electron';

module.exports = (socket: SocketIO.Server, app: Electron.App) => {

    socket.on('appQuit', () => {
        app.quit();
    });
    
    socket.on('appExit', (exitCode = 0) => {
        app.exit(exitCode);
    });

    socket.on('appRelaunch', (options) => {
        app.relaunch(options);
    });

    socket.on('appFocus', () => {
        app.focus();
    });

    socket.on('appHide', () => {
        app.hide();
    });

    socket.on('appShow', () => {
        app.show();
    });
    
    socket.on('appGetAppPath', () => {
        const path = app.getAppPath();
        socket.emit('appGetAppPathCompleted', path);
    });

    socket.on('appGetPath', (name) => {
        const path = app.getPath(name);
        socket.emit('appGetPathCompleted', path);        
    });        
}