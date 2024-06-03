import { Socket } from 'net';
let electronSocket;

export = (socket: Socket) => {
    electronSocket = socket;

    socket.on('process-execPath', () => {
        const value = process.execPath;
        electronSocket.emit('process-execPath-Completed', value);
    });

    socket.on('process-argv', () => {
        const value = process.argv;
        electronSocket.emit('process-argv-Completed', value);
    });

    socket.on('process-type', () => {
        const value = process.type;
        electronSocket.emit('process-type-Completed', value);
    });

    socket.on('process-versions', () => {
        const value = process.versions;
        electronSocket.emit('process-versions-Completed', value);
    });

    socket.on('process-defaultApp', () => {
        if (process.defaultApp === undefined) {
            electronSocket.emit('process-defaultApp-Completed', false);
            return;
        }
        electronSocket.emit('process-defaultApp-Completed', process.defaultApp);
    });

    socket.on('process-isMainFrame', () => {
        if (process.isMainFrame === undefined) {
            electronSocket.emit('process-isMainFrame-Completed', false);
            return;
        }
        electronSocket.emit('process-isMainFrame-Completed', process.isMainFrame);        
    });

    socket.on('process-resourcesPath', () => {
        const value = process.resourcesPath;
        electronSocket.emit('process-resourcesPath-Completed', value);
    });

    socket.on('process-uptime', () => {
        let value = process.uptime();
        if (value === undefined) {
            value = -1;
        }
        electronSocket.emit('process-uptime-Completed', value);
    });

    socket.on('process-pid', () => {
        if (process.pid === undefined) {
            electronSocket.emit('process-pid-Completed', -1);
            return;
        }
        electronSocket.emit('process-pid-Completed', process.pid);
    });

    socket.on('process-arch', () => {
        const value = process.arch;
        electronSocket.emit('process-arch-Completed', value);
    });

    socket.on('process-platform', () => {
        const value = process.platform;
        electronSocket.emit('process-platform-Completed', value);
    })
};
