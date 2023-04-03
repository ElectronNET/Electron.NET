"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.browserViewMediateService = exports.browserViewApi = void 0;
const electron_1 = require("electron");
const browserViews = (global['browserViews'] = global['browserViews'] || []);
let browserView, electronSocket;
const proxyToCredentialsMap = (global['proxyToCredentialsMap'] = global['proxyToCredentialsMap'] || []);
const browserViewApi = (socket) => {
    electronSocket = socket;
    socket.on('createBrowserView', (options) => {
        if (!hasOwnChildreen(options, 'webPreferences', 'nodeIntegration')) {
            options = { ...options, webPreferences: { nodeIntegration: true, contextIsolation: false } };
        }
        browserView = new electron_1.BrowserView(options);
        browserView['id'] = browserViews.length + 1;
        if (options.proxy) {
            browserView.webContents.session.setProxy({ proxyRules: options.proxy });
        }
        if (options.proxy && options.proxyCredentials) {
            proxyToCredentialsMap[options.proxy] = options.proxyCredentials;
        }
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
exports.browserViewApi = browserViewApi;
const browserViewMediateService = (browserViewId) => {
    return getBrowserViewById(browserViewId);
};
exports.browserViewMediateService = browserViewMediateService;
function getBrowserViewById(id) {
    for (let index = 0; index < browserViews.length; index++) {
        const browserViewItem = browserViews[index];
        if (browserViewItem['id'] === id) {
            return browserViewItem;
        }
    }
}
//# sourceMappingURL=browserView.js.map