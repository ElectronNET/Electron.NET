// Electron.NET host preload script.
// Registered for every session (see main.js) and exposes a small set of
// renderer helpers as `window.electronNET` - independent of the
// nodeIntegration, contextIsolation and sandbox settings of the page.
const { contextBridge, webUtils } = require('electron');

const api = {
    // Returns the absolute file system path of a File object, e.g., one
    // obtained from a drop event (event.dataTransfer.files) or an
    // <input type="file"> element. Returns an empty string for files that
    // are not backed by the file system (e.g., created via `new File()`).
    getPathForFile(file) {
        return webUtils.getPathForFile(file);
    },
};

try {
    if (process.contextIsolated) {
        contextBridge.exposeInMainWorld('electronNET', api);
    } else {
        window.electronNET = api;
    }
} catch (err) {
    console.error('Electron.NET: unable to expose renderer helpers', err);
}
