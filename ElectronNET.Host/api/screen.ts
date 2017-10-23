import { screen } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('register-screen-display-added', (id) => {
        screen.on('display-added', (event, display) => {
            socket.emit('screen-display-added-event' + id, display);
        });
    });

    socket.on('register-screen-display-removed', (id) => {
        screen.on('display-removed', (event, display) => {
            socket.emit('screen-display-removed-event' + id, display);
        });
    });

    socket.on('register-screen-display-metrics-changed', (id) => {
        screen.on('display-metrics-changed', (event, display, changedMetrics) => {
            socket.emit('screen-display-metrics-changed-event' + id, [display, changedMetrics]);
        });
    });

    socket.on('screen-getCursorScreenPoint', () => {
        var point = screen.getCursorScreenPoint();
        socket.emit('screen-getCursorScreenPointCompleted', point);
    });

    socket.on('screen-getMenuBarHeight', () => {
        var height = screen.getMenuBarHeight();
        socket.emit('screen-getMenuBarHeightCompleted', height);
    });

    socket.on('screen-getPrimaryDisplay', () => {
        var display = screen.getPrimaryDisplay();
        socket.emit('screen-getPrimaryDisplayCompleted', display);
    });

    socket.on('screen-getAllDisplays', () => {
        var display = screen.getAllDisplays();
        socket.emit('screen-getAllDisplaysCompleted', display);
    });

    socket.on('screen-getDisplayNearestPoint', (point) => {
        var display = screen.getDisplayNearestPoint(point);
        socket.emit('screen-getDisplayNearestPointCompleted', display);
    });

    socket.on('screen-getDisplayMatching', (rectangle) => {
        var display = screen.getDisplayMatching(rectangle);
        socket.emit('screen-getDisplayMatchingCompleted', display);
    });
}