import { HubConnection  } from "@microsoft/signalr";
import { globalShortcut } from 'electron';

export = (socket: HubConnection) => {
    socket.on('globalShortcut-register', (accelerator) => {
        globalShortcut.register(accelerator, () => {
            socket.invoke('GlobalShortcutPressed', accelerator);
        });
    });

    socket.on('globalShortcut-isRegistered', (guid, accelerator) => {
        const isRegistered = globalShortcut.isRegistered(accelerator);

        socket.invoke('SendClientResponseBool', guid, isRegistered);
    });

    socket.on('globalShortcut-unregister', (accelerator) => {
        globalShortcut.unregister(accelerator);
    });

    socket.on('globalShortcut-unregisterAll', () => {
        try {
            globalShortcut.unregisterAll();
        } catch (error) { }
    });
};
