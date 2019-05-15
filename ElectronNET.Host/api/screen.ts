import { screen } from 'electron';
let electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;
    socket.on('register-screen-display-added', (id) => {
        screen.on('display-added', (event, display) => {
            electronSocket.emit('screen-display-added-event' + id, display);
        });
    });

    socket.on('register-screen-display-removed', (id) => {
        screen.on('display-removed', (event, display) => {
            electronSocket.emit('screen-display-removed-event' + id, display);
        });
    });

    socket.on('register-screen-display-metrics-changed', (id) => {
        screen.on('display-metrics-changed', (event, display, changedMetrics) => {
            electronSocket.emit('screen-display-metrics-changed-event' + id, [display, changedMetrics]);
        });
    });

    socket.on('screen-getCursorScreenPoint', () => {
        const point = screen.getCursorScreenPoint();
        electronSocket.emit('screen-getCursorScreenPointCompleted', point);
    });

    socket.on('screen-getMenuBarHeight', () => {
        const height = screen.getPrimaryDisplay().workArea;
        electronSocket.emit('screen-getMenuBarHeightCompleted', height);
    });

    socket.on('screen-getPrimaryDisplay', () => {
        const display = screen.getPrimaryDisplay();
        electronSocket.emit('screen-getPrimaryDisplayCompleted', display);
    });

    socket.on('screen-getAllDisplays', () => {
        const display = screen.getAllDisplays();
        electronSocket.emit('screen-getAllDisplaysCompleted', display);
    });

    socket.on('screen-getDisplayNearestPoint', (point) => {
        const display = screen.getDisplayNearestPoint(point);
        electronSocket.emit('screen-getDisplayNearestPointCompleted', display);
    });

    socket.on('screen-getDisplayMatching', (rectangle) => {
        const display = screen.getDisplayMatching(rectangle);
        electronSocket.emit('screen-getDisplayMatchingCompleted', display);
    });
};
