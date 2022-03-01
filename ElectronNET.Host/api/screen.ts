import { screen } from 'electron';

export = (socket: SignalR.Hub.Proxy) => {
    
    socket.on('register-screen-display-added', (id) => {
        screen.on('display-added', (event, display) => {
            socket.invoke('ScreenOnDisplayAdded', id, display);
        });
    });

    socket.on('register-screen-display-removed', (id) => {
        screen.on('display-removed', (event, display) => {
            socket.invoke('ScreenOnDisplayRemoved', id, display);
        });
    });

    socket.on('register-screen-display-metrics-changed', (id) => {
        screen.on('display-metrics-changed', (event, display, changedMetrics) => {
            socket.invoke('ScreenOnDisplayMetricsChanged', id, [display, changedMetrics]);
        });
    });

    socket.on('screen-getCursorScreenPoint', (guid) => {
        const point = screen.getCursorScreenPoint();
        socket.invoke('SendClientResponseJObject', guid, point);
    });

    socket.on('screen-getMenuBarHeight', (guid) => {
        const height = screen.getPrimaryDisplay().workArea;
        socket.invoke('SendClientResponseInt', guid, height);
    });

    socket.on('screen-getPrimaryDisplay', (guid) => {
        const display = screen.getPrimaryDisplay();
        socket.invoke('SendClientResponseJObject', guid, display);
    });

    socket.on('screen-getAllDisplays', (guid) => {
        const display = screen.getAllDisplays();
        socket.invoke('SendClientResponseJArray', guid, display);
    });

    socket.on('screen-getDisplayNearestPoint', (guid, point) => {
        const display = screen.getDisplayNearestPoint(point);
        socket.invoke('SendClientResponseJObject', guid, display);
    });

    socket.on('screen-getDisplayMatching', (guid, rectangle) => {
        const display = screen.getDisplayMatching(rectangle);
        socket.invoke('SendClientResponseJObject', guid, display);
    });
};
