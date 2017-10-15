import { BrowserWindow } from "electron";
let windows: Electron.BrowserWindow[] = []
let ipc;

module.exports = (socket: SocketIO.Server) => {
    socket.on('createBrowserWindow', (options, loadUrl) => {
        let window = new BrowserWindow(options);

        window.on('closed', (sender) => {
            for (var index = 0; index < windows.length; index++) {
                var windowItem = windows[index];
                try {
                    windowItem.id;
                } catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);
                    }
                }
            }
        });

        // TODO: IPC Lösung für mehrere Fenster finden 
        if (ipc == undefined) {
            ipc = require('./ipc')(socket, window);
        }

        if (loadUrl) {
            window.loadURL(loadUrl);
        }

        windows.push(window);
        socket.emit('BrowserWindowCreated', window.id);
    });

    socket.on('browserWindow-minimize', (id) => {
        getWindowById(id).minimize();
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        for (var index = 0; index < windows.length; index++) {
            var element = windows[index];
            if (element.id == id) {
                return element;
            }
        }
    }
}