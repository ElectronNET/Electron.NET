import { HubConnection  } from "@microsoft/signalr";
import { shell } from 'electron';

export = (socket: HubConnection) => {

    socket.on('shell-showItemInFolder', (guid, fullPath) => {
        shell.showItemInFolder(fullPath);

        socket.invoke('SendClientResponseBool', guid, true);
    });

    socket.on('shell-openPath', async (guid, path) => {
        const errorMessage = await shell.openPath(path);

        socket.invoke('SendClientResponseString', guid, errorMessage);
    });

    socket.on('shell-openExternal', async (guid, url) => {
        let result = '';

        await shell.openExternal(url).catch((e) => {
            result = e.message;
        });

        socket.invoke('SendClientResponseString', guid, result);
    });

    socket.on('shell-openExternal-options', async (guid, url, options) => {
        let result = '';

        await shell.openExternal(url, options).catch(e => {
            result = e.message;
        });
        
        socket.invoke('SendClientResponseString', guid, result);
    });    

    socket.on('shell-trashItem', async (guid, fullPath, deleteOnFail) => {
        let success = false;

        try {
            await shell.trashItem(fullPath);
            success = true;
        } catch (error) {
            success = false;    
        }

        socket.invoke('SendClientResponseBool', guid, success);
    });

    socket.on('shell-beep', () => {
        shell.beep();
    });

    socket.on('shell-writeShortcutLink', (guid, shortcutPath, operation, options) => {
        const success = shell.writeShortcutLink(shortcutPath, operation, options);

        socket.invoke('SendClientResponseBool', guid, success);
    });

    socket.on('shell-readShortcutLink', (guid, shortcutPath) => {
        const shortcutDetails = shell.readShortcutLink(shortcutPath);

        socket.invoke('SendClientResponseJObject', guid, shortcutDetails);
    });
};
