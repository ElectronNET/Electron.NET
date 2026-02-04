"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on("register-screen-display-added", (id) => {
        electron_1.screen.on("display-added", (event, display) => {
            electronSocket.emit("screen-display-added" + id, display);
        });
    });
    socket.on("register-screen-display-removed", (id) => {
        electron_1.screen.on("display-removed", (event, display) => {
            electronSocket.emit("screen-display-removed" + id, display);
        });
    });
    socket.on("register-screen-display-metrics-changed", (id) => {
        electron_1.screen.on("display-metrics-changed", (event, display, changedMetrics) => {
            electronSocket.emit("screen-display-metrics-changed" + id, [
                display,
                changedMetrics,
            ]);
        });
    });
    socket.on("screen-getCursorScreenPoint", () => {
        const point = electron_1.screen.getCursorScreenPoint();
        electronSocket.emit("screen-getCursorScreenPoint-completed", point);
    });
    socket.on("screen-getMenuBarWorkArea", () => {
        const height = electron_1.screen.getPrimaryDisplay().workArea;
        electronSocket.emit("screen-getMenuBarWorkArea-completed", height);
    });
    socket.on("screen-getPrimaryDisplay", () => {
        const display = electron_1.screen.getPrimaryDisplay();
        electronSocket.emit("screen-getPrimaryDisplay-completed", display);
    });
    socket.on("screen-getAllDisplays", () => {
        const display = electron_1.screen.getAllDisplays();
        electronSocket.emit("screen-getAllDisplays-completed", display);
    });
    socket.on("screen-getDisplayNearestPoint", (point) => {
        const display = electron_1.screen.getDisplayNearestPoint(point);
        electronSocket.emit("screen-getDisplayNearestPoint-completed", display);
    });
    socket.on("screen-getDisplayMatching", (rectangle) => {
        const display = electron_1.screen.getDisplayMatching(rectangle);
        electronSocket.emit("screen-getDisplayMatching-completed", display);
    });
};
//# sourceMappingURL=screen.js.map