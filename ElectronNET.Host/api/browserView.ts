import { BrowserView } from 'electron';
let browserViews: Electron.BrowserView[] = [];
let browserView: BrowserView, electronSocket;

export = (socket: SocketIO.Socket) => {
    electronSocket = socket;

    socket.on('createBrowserView', (options) => {
        if (!hasOwnChildreen(options, 'webPreferences', 'nodeIntegration')) {
            options = { ...options, webPreferences: { nodeIntegration: true } };
        }

        browserView = new BrowserView(options);
        browserViews.push(browserView);

        electronSocket.emit('BrowserViewCreated', browserView.id);
    });

    socket.on('browserView-isDestroyed', (id) => {
        const isDestroyed = getBrowserViewById(id).isDestroyed();

        electronSocket.emit('browserView-isDestroyed-reply', isDestroyed);
    });

    socket.on('browserView-getBounds', (id) => {
        const bounds = getBrowserViewById(id).getBounds();

        electronSocket.emit('browserView-getBounds-reply', bounds);
    });

    socket.on('browserView-setBounds', (id, bounds) => {
        getBrowserViewById(id).setBounds(bounds);
    });

    socket.on('browserView-destroy', (id) => {
        const browserViewIndex = browserViews.findIndex(b => b.id === id);
        getBrowserViewById(id).destroy();
        browserViews.splice(browserViewIndex, 1);
    });

    socket.on('browserView-setAutoResize', (id, options) => {
        getBrowserViewById(id).setAutoResize(options);
    });

    socket.on('browserView-setBackgroundColor', (id, color) => {
        getBrowserViewById(id).setBackgroundColor(color);
    });

    function hasOwnChildreen(obj, ...childNames) {
        for (let i = 0; i < childNames.length; i++) {
            if (!obj || !obj.hasOwnProperty(childNames[i])) {
                return false;
            }
            obj = obj[childNames[i]];
        }

        return true;
    }

    function getBrowserViewById(id: number) {
        for (let index = 0; index < browserViews.length; index++) {
            const browserViewItem = browserViews[index];
            if (browserViewItem.id === id) {
                return browserViewItem;
            }
        }
    }
};
