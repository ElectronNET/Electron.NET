import { BrowserView } from 'electron';
const browserViews: BrowserView[] = (global['browserViews'] = global['browserViews'] || []) as BrowserView[];
let browserView: BrowserView, electronSocket;

const browserViewApi = (socket: SocketIO.Socket) => {
    electronSocket = socket;

    socket.on('createBrowserView', (options) => {
        if (!hasOwnChildreen(options, 'webPreferences', 'nodeIntegration')) {
            options = { ...options, webPreferences: { nodeIntegration: true } };
        }

        browserView = new BrowserView(options);
        browserView['id'] = browserViews.length + 1;
        browserViews.push(browserView);

        electronSocket.emit('BrowserViewCreated', browserView['id']);
    });

    socket.on('browserView-getBounds', (id) => {
        const bounds = getBrowserViewById(id).getBounds();

        electronSocket.emit('browserView-getBounds-reply', bounds);
    });

    socket.on('browserView-setBounds', (id, bounds) => {
        getBrowserViewById(id).setBounds(bounds);
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
};

const browserViewMediateService = (browserViewId: number): BrowserView => {
    return getBrowserViewById(browserViewId);
};

function getBrowserViewById(id: number) {
    for (let index = 0; index < browserViews.length; index++) {
        const browserViewItem = browserViews[index];
        if (browserViewItem['id'] === id) {
            return browserViewItem;
        }
    }
}

export { browserViewApi, browserViewMediateService };
