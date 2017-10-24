import { globalShortcut } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('globalShortcut-register', (accelerator) => {
        globalShortcut.register(accelerator, () => {
            socket.emit('globalShortcut-pressed', accelerator);
        });
    });

    socket.on('globalShortcut-isRegistered', (accelerator) => {
        const isRegistered = globalShortcut.isRegistered(accelerator);

        socket.emit('globalShortcut-isRegisteredCompleted', isRegistered);
    });

    socket.on('globalShortcut-unregister', (accelerator) => {
        globalShortcut.unregister(accelerator);
    });

    socket.on('globalShortcut-unregisterAll', () => {
        try {
            globalShortcut.unregisterAll();            
        } catch (error) { }
    });
}