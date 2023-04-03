import { App, globalShortcut } from "electron";
import { Socket } from "socket.io";

let electronSocket: Socket;

export default function connectApi(socket: Socket, app: App) {
  electronSocket = socket;

  socket.on("globalShortcut-register", (accelerator) => {
    globalShortcut.register(accelerator, () => {
      electronSocket.emit("globalShortcut-pressed", accelerator);
    });
  });

  socket.on("globalShortcut-isRegistered", (accelerator) => {
    const isRegistered = globalShortcut.isRegistered(accelerator);

    electronSocket.emit("globalShortcut-isRegisteredCompleted", isRegistered);
  });

  socket.on("globalShortcut-unregister", (accelerator) => {
    globalShortcut.unregister(accelerator);
  });

  socket.on("globalShortcut-unregisterAll", () => {
    try {
      globalShortcut.unregisterAll();
    } catch (error) {}
  });
}
