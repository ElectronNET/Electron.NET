import { ipcMain, BrowserWindow, BrowserView, Menu } from 'electron';
import { Socket } from 'net';
let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;
    socket.on('registerIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerSyncIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            const x = <any>socket;
            x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });

            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerOnceIpcMainChannel', (channel) => {
        ipcMain.once(channel, (event, args) => {
            electronSocket.emit(channel, [event.preventDefault(), args]);
        });
    });

    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        ipcMain.removeAllListeners(channel);
    });

    socket.on('sendToIpcRenderer', (browserWindow, channel, data) => {
        const window = BrowserWindow.fromId(browserWindow.id);

        if (window) {
            window.webContents.send(channel, ...data);
        }
    });

    socket.on('sendToIpcRendererBrowserView', (id, channel, data) => {
        const browserViews: BrowserView[] = (global['browserViews'] = global['browserViews'] || []) as BrowserView[];
        let view: BrowserView = null;
        for (let i = 0; i < browserViews.length; i++) {
            if (browserViews[i]['id'] === id) {
                view = browserViews[i];
                break;
            }
        }

        if (view) {
            view.webContents.send(channel, ...data);
        }
    });

    socket.on('registerHandleIpcMainChannel', (channel) => {
        ipcMain.handle(channel, (event, args) => {
            return new Promise((resolve, _reject) => {
                socket.removeAllListeners(channel + 'Handle');
                socket.on(channel + 'Handle', (result) => {
                    resolve(result);
                });
                electronSocket.emit(channel, [event.preventDefault(), args]);
            });
        });
    });

    socket.on('registerHandleOnceIpcMainChannel', (channel) => {
        ipcMain.handleOnce(channel, (event, args) => {
            return new Promise((resolve, _reject) => {
                socket.removeAllListeners(channel + 'HandleOnce');
                socket.once(channel + 'HandleOnce', (result) => {
                    resolve(result);
                });
                electronSocket.emit(channel, [event.preventDefault(), args]);
            });
        });
    });

    socket.on('removeHandlerIpcMainChannel', (channel) => {
        ipcMain.removeHandler(channel);
    });

    // Integration helpers: programmatically click menu items from renderer tests
    ipcMain.on('integration-click-application-menu', (event, id: string) => {
        try {
            const menu = Menu.getApplicationMenu();
            const mi = menu ? menu.getMenuItemById(id) : null;
            if (mi && typeof (mi as any).click === 'function') {
                const bw = BrowserWindow.fromWebContents(event.sender);
                (mi as any).click(undefined, bw, undefined);
            }
        } catch { /* ignore */ }
    });

    ipcMain.on('integration-click-context-menu', (event, windowId: number, id: string) => {
        try {
            const entries = (global as any)['contextMenuItems'] || [];
            const entry = entries.find((x: any) => x.browserWindowId === windowId);
            const mi = entry?.menu?.items?.find((i: any) => i.id === id);
            if (mi && typeof (mi as any).click === 'function') {
                const bw = BrowserWindow.fromId(windowId);
                (mi as any).click(undefined, bw, undefined);
            }
        } catch { /* ignore */ }
    });
};
