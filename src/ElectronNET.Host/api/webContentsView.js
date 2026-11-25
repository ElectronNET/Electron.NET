"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.webContentsViewMediateService = exports.webContentsViewApi = void 0;
const electron_1 = require("electron");
const webContentsViews = (global["webContentsViews"] =
    global["webContentsViews"] || []);
const proxyToCredentialsMap = (global["proxyToCredentialsMap"] = global["proxyToCredentialsMap"] || []);
let electronSocket;
const webContentsViewApi = (socket) => {
    electronSocket = socket;
    socket.on("createWebContentsView", (options) => {
        options = options || {};
        if (!hasOwnChildren(options, "webPreferences", "nodeIntegration")) {
            options = {
                ...options,
                webPreferences: {
                    ...(options.webPreferences || {}),
                    nodeIntegration: true,
                    contextIsolation: false,
                },
            };
        }
        const { proxy, proxyCredentials, ...viewOptions } = options;
        const view = new electron_1.WebContentsView(viewOptions);
        view["id"] = webContentsViews.length + 1;
        if (proxy) {
            view.webContents.session.setProxy({ proxyRules: proxy });
        }
        if (proxy && proxyCredentials) {
            proxyToCredentialsMap[proxy] = proxyCredentials;
        }
        webContentsViews.push(view);
        electronSocket.emit("WebContentsViewCreated", view["id"]);
    });
    socket.on("webContentsView-getBounds", (id) => {
        const bounds = getWebContentsViewById(id)?.getBounds();
        electronSocket.emit("webContentsView-getBounds-completed", bounds);
    });
    socket.on("webContentsView-setBounds", (id, bounds) => {
        getWebContentsViewById(id)?.setBounds(bounds);
    });
    socket.on("webContentsView-setBackgroundColor", (id, color) => {
        getWebContentsViewById(id)?.setBackgroundColor(color);
    });
    socket.on("webContentsView-setVisible", (id, visible) => {
        getWebContentsViewById(id)?.setVisible(visible);
    });
    socket.on("webContentsView-setBorderRadius", (id, radius) => {
        // setBorderRadius is only available in newer Electron versions.
        const view = getWebContentsViewById(id);
        if (view && typeof view.setBorderRadius === "function") {
            view.setBorderRadius(radius);
        }
    });
    function hasOwnChildren(obj, ...childNames) {
        for (let i = 0; i < childNames.length; i++) {
            if (!obj || !obj.hasOwnProperty(childNames[i])) {
                return false;
            }
            obj = obj[childNames[i]];
        }
        return true;
    }
};
exports.webContentsViewApi = webContentsViewApi;
const webContentsViewMediateService = (id) => {
    return getWebContentsViewById(id);
};
exports.webContentsViewMediateService = webContentsViewMediateService;
function getWebContentsViewById(id) {
    for (let index = 0; index < webContentsViews.length; index++) {
        const item = webContentsViews[index];
        if (item["id"] === id) {
            return item;
        }
    }
}
//# sourceMappingURL=webContentsView.js.map