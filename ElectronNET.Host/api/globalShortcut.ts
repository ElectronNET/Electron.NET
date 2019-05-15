import { globalShortcut } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('globalShortcut-register', (accelerator) => {
        globalShortcut.register(accelerator, () => {
            electronSocket.emit('globalShortcut-pressed', accelerator);
        });
    });

    socket.on('globalShortcut-isRegistered', (accelerator) => {
        const isRegistered = globalShortcut.isRegistered(accelerator);

        electronSocket.emit('globalShortcut-isRegisteredCompleted', isRegistered);
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
