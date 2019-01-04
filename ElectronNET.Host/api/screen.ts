import { screen } from 'electron';

export = (socket: SocketIO.Socket) => {
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
        const point = screen.getCursorScreenPoint();
        socket.emit('screen-getCursorScreenPointCompleted', point);
    });

    socket.on('screen-getMenuBarHeight', () => {
        const height = screen.getPrimaryDisplay().workArea;
        socket.emit('screen-getMenuBarHeightCompleted', height);
    });

    socket.on('screen-getPrimaryDisplay', () => {
        const display = screen.getPrimaryDisplay();
        socket.emit('screen-getPrimaryDisplayCompleted', display);
    });

    socket.on('screen-getAllDisplays', () => {
        const display = screen.getAllDisplays();
        socket.emit('screen-getAllDisplaysCompleted', display);
    });

    socket.on('screen-getDisplayNearestPoint', (point) => {
        const display = screen.getDisplayNearestPoint(point);
        socket.emit('screen-getDisplayNearestPointCompleted', display);
    });

    socket.on('screen-getDisplayMatching', (rectangle) => {
        const display = screen.getDisplayMatching(rectangle);
        socket.emit('screen-getDisplayMatchingCompleted', display);
    });
};
