"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const electron_1 = require("electron");
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
    socket.on('webContents-printToPDF', (id, options = {}, path) => __awaiter(void 0, void 0, void 0, function* () {
        const buffer = yield getWindowById(id).webContents.printToPDF(options);
        fs.writeFile(path, buffer, (error) => {
            if (error) {
                electronSocket.emit('webContents-printToPDF-completed', false);
            }
            else {
                electronSocket.emit('webContents-printToPDF-completed', true);
            }
        });
    }));
    socket.on('webContents-getUrl', function (id) {
        const browserWindow = getWindowById(id);
        electronSocket.emit('webContents-getUrl' + id, browserWindow.webContents.getURL());
    });
    socket.on('webContents-session-allowNTLMCredentialsForDomains', (id, domains) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.allowNTLMCredentialsForDomains(domains);
    });
    socket.on('webContents-session-clearAuthCache', (id, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.clearAuthCache(options);
        electronSocket.emit('webContents-session-clearAuthCache-completed' + guid);
    }));
    socket.on('webContents-session-clearCache', (id, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.clearCache();
        electronSocket.emit('webContents-session-clearCache-completed' + guid);
    }));
    socket.on('webContents-session-clearHostResolverCache', (id, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.clearHostResolverCache();
        electronSocket.emit('webContents-session-clearHostResolverCache-completed' + guid);
    }));
    socket.on('webContents-session-clearStorageData', (id, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.clearStorageData({});
        electronSocket.emit('webContents-session-clearStorageData-completed' + guid);
    }));
    socket.on('webContents-session-clearStorageData-options', (id, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.clearStorageData(options);
        electronSocket.emit('webContents-session-clearStorageData-options-completed' + guid);
    }));
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
    socket.on('webContents-session-getBlobData', (id, identifier, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        const buffer = yield browserWindow.webContents.session.getBlobData(identifier);
        electronSocket.emit('webContents-session-getBlobData-completed' + guid, buffer.buffer);
    }));
    socket.on('webContents-session-getCacheSize', (id, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        const size = yield browserWindow.webContents.session.getCacheSize();
        electronSocket.emit('webContents-session-getCacheSize-completed' + guid, size);
    }));
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
    socket.on('webContents-session-resolveProxy', (id, url, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        const proxy = yield browserWindow.webContents.session.resolveProxy(url);
        electronSocket.emit('webContents-session-resolveProxy-completed' + guid, proxy);
    }));
    socket.on('webContents-session-setDownloadPath', (id, path) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setDownloadPath(path);
    });
    socket.on('webContents-session-setPreloads', (id, preloads) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setPreloads(preloads);
    });
    socket.on('webContents-session-setProxy', (id, configuration, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const browserWindow = getWindowById(id);
        yield browserWindow.webContents.session.setProxy(configuration);
        electronSocket.emit('webContents-session-setProxy-completed' + guid);
    }));
    socket.on('webContents-session-setUserAgent', (id, userAgent, acceptLanguages) => {
        const browserWindow = getWindowById(id);
        browserWindow.webContents.session.setUserAgent(userAgent, acceptLanguages);
    });
    function getWindowById(id) {
        return electron_1.BrowserWindow.fromId(id);
    }
};
//# sourceMappingURL=webContents.js.map