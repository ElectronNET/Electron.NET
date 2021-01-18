"use strict";
const electron_1 = require("electron");
const browserView_1 = require("./browserView");
const fs = require('fs');
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('register-webContents-crashed', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            electronSocket.emit('webContents-crashed' + id, killed);
        });
    });
    socket.on('register-webContents-didFinishLoad', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('did-finish-load');
        browserWindow.webContents.on('did-finish-load', () => {
            electronSocket.emit('webContents-didFinishLoad' + id);
        });
    });
    socket.on('webContentsOpenDevTools', (id, options) => {
        if (options) {
            getWindowById(id).webContents.openDevTools(options);
        }
        else {
            getWindowById(id).webContents.openDevTools();
        }
    });
    socket.on('webContents-getPrinters', async (id) => {
        const printers = await getWindowById(id).webContents.getPrinters();
        electronSocket.emit('webContents-getPrinters-completed', printers);
    });
    socket.on('webContents-print', async (id, options = {}) => {
        await getWindowById(id).webContents.print(options);
        electronSocket.emit('webContents-print-completed', true);
    });
    socket.on('webContents-printToPDF', async (id, options = {}, path) => {
        const buffer = await getWindowById(id).webContents.printToPDF(options);
        fs.writeFile(path, buffer, (error) => {
            if (error) {
                electronSocket.emit('webContents-printToPDF-completed', false);
            }
            else {
                electronSocket.emit('webContents-printToPDF-completed', true);
            }
        });
    });
    socket.on('webContents-getUrl', function (id) {
        const browserWindow = getWindowById(id);
        electronSocket.emit('webContents-getUrl' + id, browserWindow.webContents.getURL());
    });
    socket.on('webContents-session-allowNTLMCredentialsForDomains', (id, domains) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.allowNTLMCredentialsForDomains(domains);
    });
    socket.on('webContents-session-clearAuthCache', async (id, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearAuthCache();
        electronSocket.emit('webContents-session-clearAuthCache-completed' + guid);
    });
    socket.on('webContents-session-clearCache', async (id, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearCache();
        electronSocket.emit('webContents-session-clearCache-completed' + guid);
    });
    socket.on('webContents-session-clearHostResolverCache', async (id, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearHostResolverCache();
        electronSocket.emit('webContents-session-clearHostResolverCache-completed' + guid);
    });
    socket.on('webContents-session-clearStorageData', async (id, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearStorageData({});
        electronSocket.emit('webContents-session-clearStorageData-completed' + guid);
    });
    socket.on('webContents-session-clearStorageData-options', async (id, options, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearStorageData(options);
        electronSocket.emit('webContents-session-clearStorageData-options-completed' + guid);
    });
    socket.on('webContents-session-createInterruptedDownload', (id, options) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.createInterruptedDownload(options);
    });
    socket.on('webContents-session-disableNetworkEmulation', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.disableNetworkEmulation();
    });
    socket.on('webContents-session-enableNetworkEmulation', (id, options) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.enableNetworkEmulation(options);
    });
    socket.on('webContents-session-flushStorageData', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.flushStorageData();
    });
    socket.on('webContents-session-getBlobData', async (id, identifier, guid) => {
        const browserWindow = getWindowById(id);
        const buffer = await browserWindow.webContents.session.getBlobData(identifier);
        electronSocket.emit('webContents-session-getBlobData-completed' + guid, buffer.buffer);
    });
    socket.on('webContents-session-getCacheSize', async (id, guid) => {
        const browserWindow = getWindowById(id);
        const size = await browserWindow.webContents.session.getCacheSize();
        electronSocket.emit('webContents-session-getCacheSize-completed' + guid, size);
    });
    socket.on('webContents-session-getPreloads', (id, guid) => {
        const browserWindow = getWindowById(id);
        const preloads = browserWindow.webContents.session.getPreloads();
        electronSocket.emit('webContents-session-getPreloads-completed' + guid, preloads);
    });
    socket.on('webContents-session-getUserAgent', (id, guid) => {
        const browserWindow = getWindowById(id);
        const userAgent = browserWindow.webContents.session.getUserAgent();
        electronSocket.emit('webContents-session-getUserAgent-completed' + guid, userAgent);
    });
    socket.on('webContents-session-resolveProxy', async (id, url, guid) => {
        const browserWindow = getWindowById(id);
        const proxy = await browserWindow.webContents.session.resolveProxy(url);
        electronSocket.emit('webContents-session-resolveProxy-completed' + guid, proxy);
    });
    socket.on('webContents-session-setDownloadPath', (id, path) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setDownloadPath(path);
    });
    socket.on('webContents-session-setPreloads', (id, preloads) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setPreloads(preloads);
    });
    socket.on('webContents-session-setProxy', async (id, configuration, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.setProxy(configuration);
        electronSocket.emit('webContents-session-setProxy-completed' + guid);
    });
    socket.on('webContents-session-setUserAgent', (id, userAgent, acceptLanguages) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setUserAgent(userAgent, acceptLanguages);
    });
    socket.on('register-webContents-session-cookies-changed', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.cookies.removeAllListeners('changed');
        browserWindow.webContents.session.cookies.on('changed', (event, cookie, cause, removed) => {
            electronSocket.emit('webContents-session-cookies-changed' + id, [cookie, cause, removed]);
        });
    });
    socket.on('webContents-session-cookies-get', async (id, filter, guid) => {
        const browserWindow = getWindowById(id);
        const cookies = await browserWindow.webContents.session.cookies.get(filter);
        electronSocket.emit('webContents-session-cookies-get-completed' + guid, cookies);
    });
    socket.on('webContents-session-cookies-set', async (id, details, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.set(details);
        electronSocket.emit('webContents-session-cookies-set-completed' + guid);
    });
    socket.on('webContents-session-cookies-remove', async (id, url, name, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.remove(url, name);
        electronSocket.emit('webContents-session-cookies-remove-completed' + guid);
    });
    socket.on('webContents-session-cookies-flushStore', async (id, guid) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.flushStore();
        electronSocket.emit('webContents-session-cookies-flushStore-completed' + guid);
    });
    socket.on('webContents-loadURL', (id, url, options) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.loadURL(url, options).then(() => {
            electronSocket.emit('webContents-loadURL-complete' + id);
        }).catch((error) => {
            console.error(error);
            electronSocket.emit('webContents-loadURL-error' + id, error);
        });
    });
    function getWindowById(id) {
        if (id >= 1000) {
            return browserView_1.browserViewMediateService(id - 1000);
        }
        return electron_1.BrowserWindow.fromId(id);
    }
};
//# sourceMappingURL=webContents.js.map