"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('register-screen-display-added', (id) => {
        electron_1.screen.on('display-added', (event, display) => {
            socket.invoke('ScreenOnDisplayAdded', id, display);
        });
    });
    socket.on('register-screen-display-removed', (id) => {
        electron_1.screen.on('display-removed', (event, display) => {
            socket.invoke('ScreenOnDisplayRemoved', id, display);
        });
    });
    socket.on('register-screen-display-metrics-changed', (id) => {
        electron_1.screen.on('display-metrics-changed', (event, display, changedMetrics) => {
            socket.invoke('ScreenOnDisplayMetricsChanged', id, [display, changedMetrics]);
        });
    });
    socket.on('screen-getCursorScreenPoint', (guid) => {
        const point = electron_1.screen.getCursorScreenPoint();
        socket.invoke('SendClientResponseJObject', guid, point);
    });
    socket.on('screen-getMenuBarHeight', (guid) => {
        const height = electron_1.screen.getPrimaryDisplay().workArea;
        socket.invoke('SendClientResponseString', guid, height);
    });
    socket.on('screen-getPrimaryDisplay', (guid) => {
        const display = electron_1.screen.getPrimaryDisplay();
        socket.invoke('SendClientResponseJObject', guid, display);
    });
    socket.on('screen-getAllDisplays', (guid) => {
        const display = electron_1.screen.getAllDisplays();
        socket.invoke('SendClientResponseJArray', guid, display);
    });
    socket.on('screen-getDisplayNearestPoint', (guid, point) => {
        const display = electron_1.screen.getDisplayNearestPoint(point);
        socket.invoke('SendClientResponseJObject', guid, display);
    });
    socket.on('screen-getDisplayMatching', (guid, rectangle) => {
        const display = electron_1.screen.getDisplayMatching(rectangle);
        socket.invoke('SendClientResponseJObject', guid, display);
    });
};
//# sourceMappingURL=screen.js.map