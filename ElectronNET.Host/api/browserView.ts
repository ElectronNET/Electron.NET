import { BrowserView } from 'electron';
const browserViews: BrowserView[] = (global['browserViews'] = global['browserViews'] || []) as BrowserView[];
let browserView: BrowserView;
const proxyToCredentialsMap: { [proxy: string]: string } = (global['proxyToCredentialsMap'] = global['proxyToCredentialsMap'] || []) as { [proxy: string]: string };

const browserViewApi = (socket: SignalR.Hub.Proxy) => {

    socket.on('createBrowserView', (guid, options) => {
        if (!hasOwnChildreen(options, 'webPreferences', 'nodeIntegration')) {
            options = { ...options, webPreferences: { nodeIntegration: true, contextIsolation: false } };
        }

        browserView = new BrowserView(options);
        browserView['id'] = browserViews.length + 1;

        if (options.proxy) {
            browserView.webContents.session.setProxy({proxyRules: options.proxy});
        }

        if (options.proxy && options.proxyCredentials) {
            proxyToCredentialsMap[options.proxy] = options.proxyCredentials;
        }

        browserViews.push(browserView);

        socket.invoke('SendClientResponseString', guid, browserView['id']);
    });

    socket.on('browserView-getBounds', (guid, id) => {
        const bounds = getBrowserViewById(id).getBounds();
        socket.invoke('SendClientResponseJObject', guid, bounds);
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
