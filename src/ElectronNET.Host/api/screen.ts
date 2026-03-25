import type { Socket } from "net";
import { screen } from "electron";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;

  socket.on("register-screen-display-added", (id) => {
    screen.on("display-added", (event, display) => {
      electronSocket.emit("screen-display-added" + id, display);
    });
  });

  socket.on("register-screen-display-removed", (id) => {
    screen.on("display-removed", (event, display) => {
      electronSocket.emit("screen-display-removed" + id, display);
    });
  });

  socket.on("register-screen-display-metrics-changed", (id) => {
    screen.on("display-metrics-changed", (event, display, changedMetrics) => {
      electronSocket.emit("screen-display-metrics-changed" + id, [
        display,
        changedMetrics,
      ]);
    });
  });

  socket.on("screen-getCursorScreenPoint", () => {
    const point = screen.getCursorScreenPoint();
    electronSocket.emit("screen-getCursorScreenPoint-completed", point);
  });

  socket.on("screen-getMenuBarWorkArea", () => {
    const height = screen.getPrimaryDisplay().workArea;
    electronSocket.emit("screen-getMenuBarWorkArea-completed", height);
  });

  socket.on("screen-getPrimaryDisplay", () => {
    const display = screen.getPrimaryDisplay();
    electronSocket.emit("screen-getPrimaryDisplay-completed", display);
  });

  socket.on("screen-getAllDisplays", () => {
    const display = screen.getAllDisplays();
    electronSocket.emit("screen-getAllDisplays-completed", display);
  });

  socket.on("screen-getDisplayNearestPoint", (point) => {
    const display = screen.getDisplayNearestPoint(point);
    electronSocket.emit("screen-getDisplayNearestPoint-completed", display);
  });

  socket.on("screen-getDisplayMatching", (rectangle) => {
    const display = screen.getDisplayMatching(rectangle);
    electronSocket.emit("screen-getDisplayMatching-completed", display);
  });
};
