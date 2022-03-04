import { HubConnection  } from "@microsoft/signalr";
import {BrowserView, BrowserWindow, ipcMain, webContents} from 'electron';

export = (socket: HubConnection) => {

    socket.on('registerIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            event.preventDefault();
            socket.invoke("IpcOnChannel", channel, [args]);
            event.returnValue = null;
        });
    });

    socket.on('registerIpcMainChannelWithId', (channel) => {
        ipcMain.on(channel, (event, args) => {
            event.preventDefault();
            let wcId = event.sender.id;
            let wc = webContents.fromId(wcId)
            let bw = BrowserWindow.fromWebContents(wc);
            if (bw) {
                socket.invoke("IpcMainChannelWithId", channel, {id: bw.id, wcId: wcId, args: [args]});
            }
            event.returnValue = null;
        });
    });

    socket.on('registerSyncIpcMainChannel', (channel) => {
        ipcMain.on(channel, (event, args) => {
            //const x = <any>socket;
            //x.removeAllListeners(channel + 'Sync');
            socket.on(channel + 'Sync', (result) => {
                event.returnValue = result;
            });
            event.preventDefault();
            socket.invoke("IpcOnChannel", channel, [event.preventDefault(), args]);
        });
    });

    socket.on('registerOnceIpcMainChannel', (guid, channel) => {
        ipcMain.once(channel, (event, args) => {
            event.preventDefault();
            socket.invoke("SendClientResponseJArray",guid, [event.preventDefault(), args]);
            event.returnValue = null;
        });
    });

    socket.on('removeAllListenersIpcMainChannel', (channel) => {
        ipcMain.removeAllListeners(channel);
    });

    socket.on('sendToIpcRenderer', (browserWindow, channel, ...data) => {
        const window = BrowserWindow.fromId(browserWindow.id);

        if (window) {
            window.webContents.send(channel, ...data);
        }
    });

    socket.on('sendToIpcRendererBrowserView', (id, channel, ...data) => {
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
};
