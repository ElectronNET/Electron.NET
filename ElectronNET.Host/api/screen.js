"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('register-screen-display-added', (id) => {
        electron_1.screen.on('display-added', (event, display) => {
            socket.emit('screen-display-added-event' + id, display);
        });
    });
    socket.on('register-screen-display-removed', (id) => {
        electron_1.screen.on('display-removed', (event, display) => {
            socket.emit('screen-display-removed-event' + id, display);
        });
    });
    socket.on('register-screen-display-metrics-changed', (id) => {
        electron_1.screen.on('display-metrics-changed', (event, display, changedMetrics) => {
            socket.emit('screen-display-metrics-changed-event' + id, [display, changedMetrics]);
        });
    });
    socket.on('screen-getCursorScreenPoint', () => {
        const point = electron_1.screen.getCursorScreenPoint();
        socket.emit('screen-getCursorScreenPointCompleted', point);
    });
    socket.on('screen-getMenuBarHeight', () => {
        const height = electron_1.screen.getPrimaryDisplay().workArea;
        socket.emit('screen-getMenuBarHeightCompleted', height);
    });
    socket.on('screen-getPrimaryDisplay', () => {
        const display = electron_1.screen.getPrimaryDisplay();
        socket.emit('screen-getPrimaryDisplayCompleted', display);
    });
    socket.on('screen-getAllDisplays', () => {
        const display = electron_1.screen.getAllDisplays();
        socket.emit('screen-getAllDisplaysCompleted', display);
    });
    socket.on('screen-getDisplayNearestPoint', (point) => {
        const display = electron_1.screen.getDisplayNearestPoint(point);
        socket.emit('screen-getDisplayNearestPointCompleted', display);
    });
    socket.on('screen-getDisplayMatching', (rectangle) => {
        const display = electron_1.screen.getDisplayMatching(rectangle);
        socket.emit('screen-getDisplayMatchingCompleted', display);
    });
};
//# sourceMappingURL=screen.js.map