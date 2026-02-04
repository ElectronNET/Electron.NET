import type { Socket } from "net";
import { powerMonitor } from "electron";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;
  socket.on("register-powerMonitor-lock-screen", () => {
    powerMonitor.on("lock-screen", () => {
      electronSocket.emit("powerMonitor-lock-screen");
    });
  });
  socket.on("register-powerMonitor-unlock-screen", () => {
    powerMonitor.on("unlock-screen", () => {
      electronSocket.emit("powerMonitor-unlock-screen");
    });
  });
  socket.on("register-powerMonitor-suspend", () => {
    powerMonitor.on("suspend", () => {
      electronSocket.emit("powerMonitor-suspend");
    });
  });
  socket.on("register-powerMonitor-resume", () => {
    powerMonitor.on("resume", () => {
      electronSocket.emit("powerMonitor-resume");
    });
  });
  socket.on("register-powerMonitor-ac", () => {
    powerMonitor.on("on-ac", () => {
      electronSocket.emit("powerMonitor-ac");
    });
  });
  socket.on("register-powerMonitor-battery", () => {
    powerMonitor.on("on-battery", () => {
      electronSocket.emit("powerMonitor-battery");
    });
  });
  socket.on("register-powerMonitor-shutdown", () => {
    powerMonitor.on("shutdown", () => {
      electronSocket.emit("powerMonitor-shutdown");
    });
  });
};
