// This script runs in the renderer process *before* the web page is loaded.
//
// Because the window is created with `contextIsolation: true` and `sandbox: true`, the page
// itself cannot reach Node.js or Electron. The preload script is the only place where
// `ipcRenderer` is available - and with `contextBridge` we decide exactly which capabilities
// the page gets. Never expose `ipcRenderer` itself; expose small, purpose-built functions.

const { contextBridge, ipcRenderer } = require("electron");

function subscribe(channel, callback) {
  // Strip the IpcRendererEvent - it should not leak into the page.
  const listener = (_event, ...args) => callback(...args);
  ipcRenderer.on(channel, listener);

  // Return an unsubscribe function.
  return () => ipcRenderer.removeListener(channel, listener);
}

contextBridge.exposeInMainWorld("dotnet", {
  // page -> .NET (fire and forget), handled by Electron.IpcMain.On("greet", ...)
  greet: (name) => ipcRenderer.send("greet", name),

  // page -> .NET -> page (request / response), handled by Electron.IpcMain.Handle(...)
  getRuntimeInfo: () => ipcRenderer.invoke("get-runtime-info"),
  add: (a, b) => ipcRenderer.invoke("add", { a, b }),

  // .NET -> page, sent via Electron.IpcMain.Send(window, ...)
  onGreeting: (callback) => subscribe("greeting", callback),
  onClock: (callback) => subscribe("clock", callback),
});

// The preload script can also prepare the page directly, e.g., once the DOM is available.
window.addEventListener("DOMContentLoaded", () => {
  document.documentElement.dataset.preloaded = "true";
});
