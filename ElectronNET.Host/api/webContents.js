"use strict";
const electron_1 = require("electron");
const browserView_1 = require("./browserView");
const fs = require('fs');
module.exports = (socket) => {
    socket.on('register-webContents-crashed', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('crashed');
        browserWindow.webContents.on('crashed', (event, killed) => {
            socket.invoke('WebContentOnCrashed', id, killed);
        });
    });
    socket.on('register-webContents-didFinishLoad', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.removeAllListeners('did-finish-load');
        browserWindow.webContents.on('did-finish-load', () => {
            socket.invoke('WebContentOnDidFinishLoad', id);
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
    socket.on('webContents-getPrinters', async (guid, id) => {
        const printers = await getWindowById(id).webContents.getPrinters();
        socket.invoke('SendClientResponseJArray', guid, printers);
    });
    socket.on('webContents-print', async (guid, id, options = {}) => {
        await getWindowById(id).webContents.print(options);
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-printToPDF', async (guid, id, options = {}, path) => {
        const buffer = await getWindowById(id).webContents.printToPDF(options);
        fs.writeFile(path, buffer, (error) => {
            if (error) {
                socket.invoke('SendClientResponseBool', guid, false);
            }
            else {
                socket.invoke('SendClientResponseBool', guid, true);
            }
        });
    });
    socket.on('webContents-getUrl', function (guid, id) {
        const browserWindow = getWindowById(id);
        socket.invoke('SendClientResponseString', guid, browserWindow.webContents.getURL());
    });
    socket.on('webContents-session-allowNTLMCredentialsForDomains', (id, domains) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.allowNTLMCredentialsForDomains(domains);
    });
    socket.on('webContents-session-clearAuthCache', async (guid, id) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearAuthCache();
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-session-clearCache', async (guid, id) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearCache();
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-session-clearHostResolverCache', async (guid, id) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearHostResolverCache();
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-session-clearStorageData', async (guid, id) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearStorageData({});
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-session-clearStorageData-options', async (guid, id, options) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.clearStorageData(options);
        socket.invoke('SendClientResponseBool', guid, true);
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
    socket.on('webContents-session-getBlobData', async (guid, id, identifier) => {
        const browserWindow = getWindowById(id);
        const buffer = await browserWindow.webContents.session.getBlobData(identifier);
        socket.invoke('SendClientResponseJArray', guid, buffer.buffer);
    });
    socket.on('webContents-session-getCacheSize', async (guid, id) => {
        const browserWindow = getWindowById(id);
        const size = await browserWindow.webContents.session.getCacheSize();
        socket.invoke('SendClientResponseInt', guid, size);
    });
    socket.on('webContents-session-getPreloads', (guid, id) => {
        const browserWindow = getWindowById(id);
        const preloads = browserWindow.webContents.session.getPreloads();
        socket.invoke('SendClientResponseJArray', guid, preloads);
    });
    socket.on('webContents-session-getUserAgent', (guid, id) => {
        const browserWindow = getWindowById(id);
        const userAgent = browserWindow.webContents.session.getUserAgent();
        socket.invoke('SendClientResponseString', guid, userAgent);
    });
    socket.on('webContents-session-resolveProxy', async (guid, id, url) => {
        const browserWindow = getWindowById(id);
        const proxy = await browserWindow.webContents.session.resolveProxy(url);
        socket.invoke('SendClientResponseString', guid, proxy);
    });
    socket.on('webContents-session-setDownloadPath', (id, path) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setDownloadPath(path);
    });
    socket.on('webContents-session-setPreloads', (id, preloads) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setPreloads(preloads);
    });
    socket.on('webContents-session-setProxy', async (guid, id, configuration) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.setProxy(configuration);
        socket.invoke('SendClientResponseBool', guid, true);
    });
    socket.on('webContents-session-setUserAgent', (id, userAgent, acceptLanguages) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setUserAgent(userAgent, acceptLanguages);
    });
    socket.on('register-webContents-session-cookies-changed', (id) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.cookies.removeAllListeners('changed');
        browserWindow.webContents.session.cookies.on('changed', (event, cookie, cause, removed) => {
            socket.invoke('CookiesOnChanged', id, [cookie, cause, removed]);
        });
    });
    socket.on('webContents-session-cookies-get', async (guid, id, filter) => {
        const browserWindow = getWindowById(id);
        const cookies = await browserWindow.webContents.session.cookies.get(filter);
        socket.invoke('SendClientResponseJArray', guid, cookies);
    });
    socket.on('webContents-session-cookies-set', async (id, details) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.set(details);
        // Needed ?
        //socket.invoke('webContents-session-cookies-set-completed', guid);
    });
    socket.on('webContents-session-cookies-remove', async (id, url, name) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.remove(url, name);
        // Needed ?
        // socket.invoke('webContents-session-cookies-remove-completed', guid);
    });
    socket.on('webContents-session-cookies-flushStore', async (id) => {
        const browserWindow = getWindowById(id);
        await browserWindow.webContents.session.cookies.flushStore();
        // Needed ?
        // socket.invoke('webContents-session-cookies-flushStore-completed' + guid);
    });
    socket.on('webContents-loadURL', (guid, id, url, options) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.loadURL(url, options).then(() => {
            socket.invoke('webContents-loadURL-complete', guid, null);
        }).catch((error) => {
            console.error(error);
            socket.invoke('webContents-loadURL-error', guid, error);
        });
    });
    socket.on('webContents-insertCSS', (id, isBrowserWindow, path) => {
        if (isBrowserWindow) {
            const browserWindow = getWindowById(id);
            if (browserWindow) {
                socket.invoke('webContents-printToPDF-completed', false);
            }
        }
        else {
            const browserViews = (global['browserViews'] = global['browserViews'] || []);
            let view = null;
            for (let i = 0; i < browserViews.length; i++) {
                if (browserViews[i]['id'] + 1000 === id) {
                    view = browserViews[i];
                    break;
                }
            }
            if (view) {
                socket.invoke('webContents-printToPDF-completed', true);
            }
        }
    });
    socket.on('webContents-session-getAllExtensions', (guid, id) => {
        const browserWindow = getWindowById(id);
        const extensionsList = browserWindow.webContents.session.getAllExtensions();
        const chromeExtensionInfo = [];
        Object.keys(extensionsList).forEach(key => {
            chromeExtensionInfo.push(extensionsList[key]);
        });
        socket.invoke('SendClientResponseJArray', guid, chromeExtensionInfo);
    });
    socket.on('webContents-session-removeExtension', (id, name) => {
        const browserWindow = getWindowById(id);
        socket.invoke('webContents-getUrl' + id, browserWindow.webContents.getURL());
    });
    socket.on('webContents-session-loadExtension', async (guid, id, path, allowFileAccess = false) => {
        const browserWindow = getWindowById(id);
        const extension = await browserWindow.webContents.session.loadExtension(path, { allowFileAccess: allowFileAccess });
        socket.invoke('SendClientResponseJObject', guid, extension);
    });
    function getWindowById(id) {
        if (id >= 1000) {
            return (0, browserView_1.browserViewMediateService)(id - 1000);
        }
        return electron_1.BrowserWindow.fromId(id);
    }
};
//# sourceMappingURL=webContents.js.map